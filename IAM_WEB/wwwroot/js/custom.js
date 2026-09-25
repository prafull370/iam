
var baseUrl = ($('base').attr('href').slice(0, -1));

$(function () {
    $('.usricn').each(function () {
        let userName = $(this).data("name"); // Corrected: "name" should be a string
        if (userName && userName.trim().length > 0) {
            let initials = userName.split(" ").map(word => word[0]).join("").toUpperCase();
            $(this).text(initials); // Ensure only the specific element is updated
        }
    });
});


function logoutAndSubmit() {
    sessionStorage.clear();
    localStorage.clear();
    document.querySelector('.logoutForm').submit();
}

// Function to replace image with initials
function replaceWithPlaceholder(imgElement) {
    let parentDiv = $(imgElement).closest('.tnp');
    let userName = parentDiv.attr('data-name') || " ";
    let initials = getInitials(userName);

    // Replace image with a div containing initials
    $(imgElement).replaceWith(`<div class="profile-placeholder">${initials}</div>`);
}

// Function to generate initials
function getInitials(name) {
    let parts = name.split(" ");
    let initials = parts[0].charAt(0).toUpperCase();
    if (parts.length > 1) {
        initials += parts[1].charAt(0).toUpperCase();
    }
    return initials;
}

/*-----------------Is Null----------------*/
function IsNullData(data) {
    return (data === null || data === '' || data === 'undefined' || data === undefined || data == "NaN") ? '' : data;
}

function IsNull(data) {
    return (data === null || data === ''
        || data === 'undefined' || data === undefined
        || data === "Infinity" || data === Infinity
        || data === "NaN" || Number.isNaN(data)) ? true : false;
}

function IsNullOrZero(data) {
    return (data === null || data === ''
        || data === 'undefined' || data === undefined
        || data === "Infinity" || data === Infinity
        || data === "NaN" || Number.isNaN(data)
        || data === 0 || data === '0') ? true : false;
}

//image error 
function imgError(image) {
    image.onerror = "";
    image.src = baseUrl + "/images/user.svg";
    return true;
}

function splitdata(str) {
    // Split the string by comma into an array of substrings
    var substrings = str.split(',');

    // Trim each substring using the map function
    var trimmedSubstrings = substrings.map(function (substring) {
        return substring.trim();
    });
    return trimmedSubstrings;
}
/*-----------------Status-------*/
function SetStatus(type) {
    var css = "";
    var text = "";
    switch (type) {
        case "Draft":
            css = "badge badge-warning";
            text = "Draft";
            break;
        case "Pending":
            css = "badge badge-primary";
            text = "Pending";
            break;
        case "InProgress":
            css = "badge badge-primary";
            text = "In-Progress";
            break;
        case "Shortlist":
            css = "badge badge-primary";
            text = "Shortlist";
            break;
        case "Accepted":
            css = "badge badge-success";
            text = "Accepted";
            break;
        case "Completed":
            css = "badge badge-success";
            text = "Completed";
            break;
        case "Success":
            css = "badge badge-success";
            text = "Success";
            break;
        case "Rejected":
            css = "badge badge-danger";
            text = "Rejected";
            break;
        case "Cancel":
            css = "badge badge-gray";
            text = "Cancel";
            break;
    }
    return { css: css, text: text };
}

(function () {
    "use strict";


    /**
     * Easy selector helper function
     */
    const select = (el, all = false) => {
        el = el.trim()
        if (all) {
            return [...document.querySelectorAll(el)]
        } else {
            return document.querySelector(el)
        }
    }

    /**
     * Easy event listener function
     */
    const on = (type, el, listener, all = false) => {
        if (all) {
            select(el, all).forEach(e => e.addEventListener(type, listener))
        } else {
            select(el, all).addEventListener(type, listener)
        }
    }

    /**
     * Easy on scroll event listener 
     */
    const onscroll = (el, listener) => {
        el.addEventListener('scroll', listener)
    }

    /**
     * Sidebar toggle
     */
    if (select('.toggle-sidebar-btn')) {
        on('click', '.toggle-sidebar-btn', function (e) {
            select('body').classList.toggle('toggle-sidebar')
        })
    }

    /**
     * Search bar toggle
     */
    if (select('.search-bar-toggle')) {
        on('click', '.search-bar-toggle', function (e) {
            select('.search-bar').classList.toggle('search-bar-show')
        })
    }

    /**
     * Navbar links active state on scroll
     */
    let navbarlinks = select('#navbar .scrollto', true)
    const navbarlinksActive = () => {
        let position = window.scrollY + 200
        navbarlinks.forEach(navbarlink => {
            if (!navbarlink.hash) return
            let section = select(navbarlink.hash)
            if (!section) return
            if (position >= section.offsetTop && position <= (section.offsetTop + section.offsetHeight)) {
                navbarlink.classList.add('active')
            } else {
                navbarlink.classList.remove('active')
            }
        })
    }
    window.addEventListener('load', navbarlinksActive)
    onscroll(document, navbarlinksActive)

    /**
     * Toggle .header-scrolled class to #header when page is scrolled
     */
    let selectHeader = select('#header')
    if (selectHeader) {
        const headerScrolled = () => {
            if (window.scrollY > 100) {
                selectHeader.classList.add('header-scrolled')
            } else {
                selectHeader.classList.remove('header-scrolled')
            }
        }
        window.addEventListener('load', headerScrolled)
        onscroll(document, headerScrolled)
    }

    /**
     * Back to top button
     */
    let backtotop = select('.back-to-top')
    if (backtotop) {
        const toggleBacktotop = () => {
            if (window.scrollY > 100) {
                backtotop.classList.add('active')
            } else {
                backtotop.classList.remove('active')
            }
        }
        window.addEventListener('load', toggleBacktotop)
        onscroll(document, toggleBacktotop)
    }

})();
//Bootstrap Tooltip
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})

$(function () {

    $(document).on('change', '.uploadfile', function (e) {
        var fname = e.target.files[0].name;
        var fileuploaded = $(this).closest('.fldv').find('.fileuploaded');
        fileuploaded.find('.file-name').text(fname);
        fileuploaded.css("display", "block");
    });

    $(document).on('click', '.removefile', function () {
        var fileuploaded = $(this).closest('.fileuploaded');
        fileuploaded.css("display", "none");
        // Clear the file input if needed
        var fileInput = fileuploaded.closest('.fldv').find('.uploadfile');
        fileInput.val(''); // Clear the file input value
    });

    $('.show-pass').on('click', function () {
        $(this).toggleClass('active');
        if ($('#Password').attr('type') === 'password') {
            $('#Password').attr('type', 'text');
        } else if ($('#Password').attr('type') === 'text') {
            $('#Password').attr('type', 'password');
        }
    });

});

window.addEventListener("pageshow", function (event) {
    var historyTraversal = event.persisted ||
        (typeof window.performance != "undefined" &&
            window.performance.navigation.type === 2);
    if (historyTraversal) {
        // Handle page restore.
        window.location.reload();
    }
});

function validate_img_file(fileName) {
    var allowed_extensions = new Array("jpg", "jpeg", "png", "gif");
    var file_extension = fileName.split('.').pop().toLowerCase();

    for (var i = 0; i <= allowed_extensions.length; i++) {
        if (allowed_extensions.includes(file_extension)) {
            return true; // valid file extension
        }
    }
    return false;
}

function validate_file(fileName) {
    var allowed_extensions = new Array("jpg", "jpeg", "png", "gif", "pdf");
    var file_extension = fileName.split('.').pop().toLowerCase();

    for (var i = 0; i <= allowed_extensions.length; i++) {
        if (allowed_extensions.includes(file_extension)) {
            return true; // valid file extension
        }
    }
    return false;
}
function validate_file_with_mp4(fileName) {
    var allowed_extensions = new Array("jpg", "jpeg", "png", "gif", "pdf", "mp4", "mpeg");
    var file_extension = fileName.split('.').pop().toLowerCase();

    for (var i = 0; i <= allowed_extensions.length; i++) {
        if (allowed_extensions.includes(file_extension)) {
            return true; // valid file extension
        }
    }
    return false;
}
function validate_file_size(file_size, fix_size = "1048576") {
    if (file_size > fix_size) {
        return false;
    }
    return true;
}

$(document).on("keypress keyup blur", ".isNumeric", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});

$(document).on("input", ".isDecimal", function (event) {
    var inputValue = $(this).val();
    // Replace non-numeric and non-decimal characters except for the first decimal point
    $(this).val(inputValue.replace(/[^\d.]/g, '').replace(/^(\d*\.?)|(\d*)\.|\./g, '$1$2'));

    // Check if the input value contains more than two decimal places
    var decimalIndex = inputValue.indexOf('.');
    if (decimalIndex !== -1 && inputValue.substring(decimalIndex + 1).length > 2) {
        // If more than two decimal places, truncate the input value to two decimal places
        $(this).val(parseFloat(inputValue).toFixed(2));
    }
});

$(document).on('keydown cut copy paste', '.calicnrgt', function () {
    return false;
});

$(document).on("select2:close", "select", function (e) {
    if ($(this).hasClass("input-validation-error") || $(this).hasClass("valid")) {
        $(this).valid();
    }
});

$(document).on('change', '.calicnrgt', function () {
    if ($(this).hasClass("input-validation-error") || $(this).hasClass("valid")) {
        $(this).removeData('previousValue').valid();
    }

    if ($(this).attr('data-val') == "true") {
        $(this).valid();
    }
    //$.validator.methods.date = function (value, element) {
    //    return this.optional(element) || moment(value, "DD-MMM-YYYY", true).isValid();
    //}
});

$('input[type="checkbox"].chkbtn').on('change', function () {
    if ($(this).is(':checked')) {
        $(this).attr('value', 'true');
    } else {
        $(this).attr('value', 'false');
    }
});


$(function () {
    setNavigation();
});

function removeUrlSlashes(url) {
    return url.replace(/^\/|\/$/g, '');
}
function setNavigation() {
    var path = removeUrlSlashes(window.location.pathname);
    path = decodeURIComponent(path);

    var activeHref = localStorage.getItem('activeHref');
    if (!IsNull(activeHref)) {
        activeHref = removeUrlSlashes(activeHref);
    }

    var menuactive = false;

    $("#sidemenu .nav-item .nav-link").each(function () {
        var href = removeUrlSlashes($(this).attr('href'));

        // Check if href is "javascript:;"
        if (href === "javascript:;") {
            $("#sidemenu .nav-item ul li a").each(function () {
                var subHref = removeUrlSlashes($(this).attr('href'));

                // Match path or activeHref with sub-menu link
                if (!menuactive && (path === subHref || activeHref === subHref)) {
                    $(this).addClass('active'); // Add active class to the current link
                    $(this).closest('.nav-item').find('.nav-content').addClass('show'); // Expand parent menu
                    $(this).closest('.nav-item').find('> .nav-link').addClass('active'); // Highlight top-level link

                    localStorage.setItem('activeHref', subHref); // Save active link in localStorage
                    menuactive = true;
                }
            });
        }

        // Check top-level navigation link
        if (!menuactive && (path === href || activeHref === href)) {
            $(this).addClass('active'); // Add active class to the current link
            localStorage.setItem('activeHref', href); // Save active link in localStorage
            menuactive = true;
        }
    });

}

$("#sidemenu").on("click", "a.menulink", function (e) {
    var activeHref = $(this).attr("href");
    localStorage.setItem('activeHref', activeHref);
});

function isValidJSON(jsonString) {
    try {
        JSON.parse(jsonString);
        return true; // Valid JSON
    } catch (e) {
        return false; // Invalid JSON
    }
}

$.ajaxSetup({
    statusCode: {
        451: function () {
            // If a 451 error status is returned, redirect to logout page
            window.location.href = '/Account/Login';
        }
    }
});

//const element = document.querySelector('.rgntvcDown');
var element = document.querySelector('.rgntvcDown');
if (element) {
    element.addEventListener('click', function (event) {
        event.preventDefault(); // Prevent default behavior
        const url = this.getAttribute('href');
        if (!url) {
            console.error('No URL found for download.');
            return;
        }
        const fileName = url.split('/').pop(); // Extract filename from URL

        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.blob();
            })
            .then(blob => {
                //const link = document.createElement('a');
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = fileName;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            })
            .catch(error => console.error('Download error:', error));
    });
}

if (document.fonts) {
    document.fonts.load('1.25rem "Ugo Icons"').then(function () {
        document.body.classList.add('fonts-loaded');
    });
}
