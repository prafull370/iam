// ============================================================================
//  Forgot Password  —  Send OTP  ->  Verify OTP  ->  Reset Password
//
//  ROUTES come from data-* attributes rendered by @Url.Action in the view, so a
//  wrong URL is impossible. No dependency on the admin layout (own toasts).
// ============================================================================
(function () {
    'use strict';

    var URLS = {};
    var loginId = '';
    var timer = null;

    // ---- read PascalCase OR camelCase (Json() emits camelCase by default) ----
    function prop(o, name) {
        if (!o) return undefined;
        if (o[name] !== undefined) return o[name];
        var lc = name.charAt(0).toLowerCase() + name.slice(1);
        if (o[lc] !== undefined) return o[lc];
        for (var k in o) {
            if (Object.prototype.hasOwnProperty.call(o, k) && k.toLowerCase() === name.toLowerCase()) return o[k];
        }
        return undefined;
    }
    var ok = function (r) { return prop(r, 'Status') === true; };
    var msg = function (r, d) { return prop(r, 'Message') || d; };

    function token() { return $('input[name="__RequestVerificationToken"]').val() || ''; }

    // ---- Toasts -------------------------------------------------------------
    function toast(text, type) {
        var icon = type === 'success' ? '\u2713' : (type === 'error' ? '\u2715' : '!');
        var $t = $('<div class="fp-toast fp-toast-' + (type || 'info') + '">' +
            '<span class="fp-toast-ico"></span>' +
            '<span class="fp-toast-msg"></span></div>');
        $t.find('.fp-toast-ico').text(icon);
        $t.find('.fp-toast-msg').text(text);
        $('#fpToasts').append($t);
        setTimeout(function () { $t.addClass('show'); }, 10);
        setTimeout(function () {
            $t.removeClass('show');
            setTimeout(function () { $t.remove(); }, 300);
        }, 4200);
    }

    // ---- UI state -----------------------------------------------------------
    function busy($btn, on) {
        $btn.prop('disabled', on).toggleClass('is-loading', on);
    }
    function goStep(n) {
        $('.fp-panel').removeClass('is-active');
        var ids = { 1: '#stepLoginId', 2: '#stepOtp', 3: '#stepReset', 4: '#stepDone' };
        $(ids[n]).addClass('is-active');

        $('.fp-step').each(function () {
            var s = +$(this).data('step');
            $(this).toggleClass('is-active', s === n).toggleClass('is-done', s < n);
        });
        $('#btnBackToLogin').toggle(n !== 4);
    }
    function fieldError(id, text) {
        $('#' + id).text(text || '').toggleClass('show', !!text);
    }

    // ---- Step 1 : send OTP --------------------------------------------------
    function sendOtp() {
        var v = ($('#txtLoginId').val() || '').trim();
        fieldError('errLoginId', '');

        if (!v) { fieldError('errLoginId', 'Please enter your email or mobile number.'); return; }
        var isEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);
        var isMobile = /^[0-9]{10}$/.test(v);
        if (!isEmail && !isMobile) {
            fieldError('errLoginId', 'Enter a valid email address or 10-digit mobile number.');
            return;
        }

        var $btn = $('#btnSendOtp');
        busy($btn, true);

        $.ajax({
            url: URLS.send,
            type: 'POST',
            data: { loginId: v },
            headers: { 'RequestVerificationToken': token() },
            success: function (res) {
                busy($btn, false);
                if (ok(res)) {
                    loginId = v;
                    $('#lblOtpSentTo').text(msg(res, 'OTP sent.'));
                    clearOtp();
                    goStep(2);
                    startTimer(60);
                    $('.fp-otp-box').first().focus();
                    toast(msg(res, 'OTP sent successfully.'), 'success');
                } else {
                    fieldError('errLoginId', msg(res, 'Failed to send OTP.'));
                    toast(msg(res, 'Failed to send OTP.'), 'error');
                }
            },
            error: function (xhr, s, e) {
                busy($btn, false);
                toast('Unable to send OTP. ' + (e || 'Please try again.'), 'error');
            }
        });
    }

    // ---- Step 2 : verify OTP ------------------------------------------------
    function collectOtp() {
        var v = '';
        $('.fp-otp-box').each(function () { v += ($(this).val() || '').trim(); });
        $('#txtOtp').val(v);
        return v;
    }
    function clearOtp() {
        $('.fp-otp-box').val('');
        $('#txtOtp').val('');
        fieldError('errOtp', '');
    }

    function verifyOtp() {
        var otp = collectOtp();
        fieldError('errOtp', '');

        if (!/^[0-9]{6}$/.test(otp)) {
            fieldError('errOtp', 'Please enter the complete 6-digit OTP.');
            return;
        }

        var $btn = $('#btnVerifyOtp');
        busy($btn, true);

        $.ajax({
            url: URLS.verify,
            type: 'POST',
            data: { loginId: loginId, otp: otp },
            headers: { 'RequestVerificationToken': token() },
            success: function (res) {
                busy($btn, false);
                if (ok(res)) {
                    stopTimer();
                    goStep(3);
                    $('#txtNewPassword').val('').focus();
                    $('#txtConfirmPassword').val('');
                    scorePassword('');
                    toast('OTP verified successfully.', 'success');
                } else {
                    fieldError('errOtp', msg(res, 'Invalid or expired OTP.'));
                    $('.fp-otp-box').addClass('is-error');
                    setTimeout(function () { $('.fp-otp-box').removeClass('is-error'); }, 600);
                    toast(msg(res, 'Invalid or expired OTP.'), 'error');
                }
            },
            error: function (xhr, s, e) {
                busy($btn, false);
                toast('Unable to verify OTP. ' + (e || 'Please try again.'), 'error');
            }
        });
    }

    // ---- Step 3 : reset password -------------------------------------------
    var RULES = {
        len: function (p) { return p.length >= 8 && p.length <= 20; },
        upper: function (p) { return /[A-Z]/.test(p); },
        lower: function (p) { return /[a-z]/.test(p); },
        digit: function (p) { return /[0-9]/.test(p); },
        special: function (p) { return /[^A-Za-z0-9]/.test(p); }
    };

    function scorePassword(p) {
        var passed = 0;
        Object.keys(RULES).forEach(function (k) {
            var good = RULES[k](p);
            if (good) passed++;
            $('#pwdRules li[data-rule="' + k + '"]').toggleClass('is-ok', good);
        });
        var pct = (passed / 5) * 100;
        var label = p.length === 0 ? '\u2014' : (passed <= 2 ? 'Weak' : (passed <= 4 ? 'Medium' : 'Strong'));
        var cls = passed <= 2 ? 'weak' : (passed <= 4 ? 'medium' : 'strong');
        $('#strengthFill').css('width', pct + '%').attr('class', cls);
        $('#strengthText').text(label).attr('class', 'fp-strength-text ' + cls);
        return passed === 5;
    }

    function resetPassword() {
        var p1 = $('#txtNewPassword').val() || '';
        var p2 = $('#txtConfirmPassword').val() || '';
        fieldError('errPassword', '');

        if (!p1 || !p2) { fieldError('errPassword', 'Please enter and confirm your new password.'); return; }
        if (!scorePassword(p1)) { fieldError('errPassword', 'Password does not meet all the requirements.'); return; }
        if (p1 !== p2) { fieldError('errPassword', 'New Password and Confirm Password do not match.'); return; }

        var $btn = $('#btnResetPassword');
        busy($btn, true);

        $.ajax({
            url: URLS.reset,
            type: 'POST',
            data: { loginId: loginId, newPassword: p1, confirmPassword: p2 },
            headers: { 'RequestVerificationToken': token() },
            success: function (res) {
                busy($btn, false);
                if (ok(res)) {
                    goStep(4);
                    toast(msg(res, 'Password reset successfully.'), 'success');
                    setTimeout(function () { window.location.href = URLS.login; }, 2500);
                } else {
                    fieldError('errPassword', msg(res, 'Failed to reset password.'));
                    toast(msg(res, 'Failed to reset password.'), 'error');
                }
            },
            error: function (xhr, s, e) {
                busy($btn, false);
                toast('Unable to reset password. ' + (e || 'Please try again.'), 'error');
            }
        });
    }

    // ---- Resend timer -------------------------------------------------------
    function startTimer(sec) {
        stopTimer();
        var left = sec;
        $('#btnResendOtp').prop('disabled', true);
        $('#lblResendTimer').text('Resend available in ' + left + 's');
        timer = setInterval(function () {
            left--;
            if (left <= 0) {
                stopTimer();
                $('#btnResendOtp').prop('disabled', false);
                $('#lblResendTimer').text('Didn\u2019t receive the code?');
            } else {
                $('#lblResendTimer').text('Resend available in ' + left + 's');
            }
        }, 1000);
    }
    function stopTimer() { if (timer) { clearInterval(timer); timer = null; } }

    // ---- Wire up ------------------------------------------------------------
    $(function () {
        var $app = $('#fpApp');
        URLS = {
            send: $app.data('send-url'),
            verify: $app.data('verify-url'),
            reset: $app.data('reset-url'),
            login: $app.data('login-url')
        };

        goStep(1);

        $('#btnSendOtp').on('click', sendOtp);
        $('#btnVerifyOtp').on('click', verifyOtp);
        $('#btnResetPassword').on('click', resetPassword);

        $('#btnResendOtp').on('click', function () {
            if ($(this).prop('disabled')) return;
            $('#txtLoginId').val(loginId);
            sendOtp();
        });

        $('#btnBackToStep1').on('click', function () {
            stopTimer();
            clearOtp();
            goStep(1);
            $('#txtLoginId').focus();
        });

        // BACK TO LOGIN — uses the Razor-generated URL, so it always resolves
        $('#btnBackToLogin, #btnGoLogin').on('click', function () {
            window.location.href = URLS.login;
        });

        // OTP boxes: auto-advance, backspace, paste
        $('.fp-otp-box')
            .on('input', function () {
                this.value = this.value.replace(/[^0-9]/g, '');
                if (this.value && $(this).next('.fp-otp-box').length) $(this).next('.fp-otp-box').focus();
                collectOtp();
            })
            .on('keydown', function (e) {
                if (e.key === 'Backspace' && !this.value && $(this).prev('.fp-otp-box').length) {
                    $(this).prev('.fp-otp-box').focus();
                }
                if (e.key === 'Enter') { e.preventDefault(); verifyOtp(); }
            })
            .on('paste', function (e) {
                var data = (e.originalEvent.clipboardData || window.clipboardData).getData('text') || '';
                var digits = data.replace(/[^0-9]/g, '').slice(0, 6).split('');
                if (!digits.length) return;
                e.preventDefault();
                $('.fp-otp-box').each(function (i) { $(this).val(digits[i] || ''); });
                collectOtp();
                $('.fp-otp-box').eq(Math.min(digits.length, 5)).focus();
            });

        // live password strength
        $('#txtNewPassword').on('input', function () { scorePassword(this.value); });

        // show / hide password
        $('.fp-eye').on('click', function () {
            var $i = $('#' + $(this).data('target'));
            var show = $i.attr('type') === 'password';
            $i.attr('type', show ? 'text' : 'password');
            $(this).toggleClass('is-on', show);
        });

        // Enter key
        $('#txtLoginId').on('keydown', function (e) {
            if (e.key === 'Enter') { e.preventDefault(); sendOtp(); }
        });
        $('#txtNewPassword, #txtConfirmPassword').on('keydown', function (e) {
            if (e.key === 'Enter') { e.preventDefault(); resetPassword(); }
        });
    });
})();
