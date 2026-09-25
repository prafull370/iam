/**
 * FluentValidation Length Validator - Client-Side Real-Time Validation
 * 1. Reads validation metadata from HTML data-val attributes
 * 2. Applies the HTML maxlength attribute to input/textarea elements
 * 3. Shows error messages when max length is reached
 * 4. Enforces maximum length by preventing additional character input
 * 5. Shows MinLength warning messages while typing
 * 7. Preserves existing validation messages without interference
 * Usage:
 * - Include this file after jQuery and jQuery Validate Unobtrusive
 * - Initialize on page load: FluentValidationLengthValidator.init()
 * - For dynamic forms: FluentValidationLengthValidator.reprocess()
 */

(function (window) {
    'use strict';

    const FluentValidationLengthValidator = {
        // Configuration
        config: {
            minLengthMessageClass: 'min-length-message',
            maxLengthMessageClass: 'max-length-message',
            errorClass: 'input-validation-error',
            validClass: 'input-validation-valid',
            // Message templates with clean, consistent styling (no background/border)
            minLengthMessageTemplate: '<small class="min-length-message" style="display:none; color: #d32f2f; font-size: 0.85em; margin-top: 4px; margin-left: 0;"></small>',
            maxLengthMessageTemplate: '<small class="max-length-message" style="display:none; color: #d32f2f; font-size: 0.85em; margin-top: 4px; margin-left: 0;"></small>',
        },

        // Store validation metadata for each field
        fieldValidationMap: {},

        /**
         * Initialize the validator - parse all existing fields and setup event delegation
         */
        init: function () {
            // Remove any leftover character counters from previous versions
            $('small.char-counter').remove();

            // Parse existing fields on page load
            this.processFields();

            // Setup event delegation for dynamically added fields
            this.setupEventDelegation();
        },

        /**
         * Setup event delegation for dynamic form elements
         * This allows new fields added to the DOM to be automatically handled
         */
        setupEventDelegation: function () {
            const self = this;

            // Event delegation: listen for input events on document level
            $(document).on('input', 'input[data-val=true], textarea[data-val=true]', function () {
                self.handleFieldInput(this);
            });

            // Observer for dynamically added fields and validation spans (MutationObserver for added elements)
            if (window.MutationObserver) {
                const observer = new MutationObserver(function (mutations) {
                    mutations.forEach(function (mutation) {
                        if (mutation.addedNodes.length) {
                            for (let node of mutation.addedNodes) {
                                if (node.nodeType !== 1) continue; // only elements

                                // If the added node itself is a form field, process it
                                if (self.isFormField(node)) {
                                    self.processField(node);
                                }

                                // If the added node is a validation message span created by unobtrusive validation
                                if (node.hasAttribute && node.hasAttribute('data-valmsg-for')) {
                                    const fieldName = node.getAttribute('data-valmsg-for');
                                    // If we already processed the corresponding input, link this span
                                    if (fieldName && self.fieldValidationMap[fieldName]) {
                                        const validationData = self.fieldValidationMap[fieldName];
                                        validationData.valMsgSpan = $(node);
                                        // Hide it initially to avoid duplicates; update state to show/hide correctly
                                        validationData.valMsgSpan.hide();
                                        const fieldElem = document.getElementsByName(fieldName)[0];
                                        if (fieldElem) {
                                            self.updateFieldState(fieldElem, validationData);
                                        }
                                    }
                                }

                                // Check children of this node for fields or validation spans
                                const fields = $(node).find('input[data-val=true], textarea[data-val=true]');
                                if (fields.length) {
                                    fields.each(function () {
                                        self.processField(this);
                                    });
                                }

                                const valMsgSpans = $(node).find('[data-valmsg-for]');
                                if (valMsgSpans.length) {
                                    valMsgSpans.each(function () {
                                        const fieldName = $(this).attr('data-valmsg-for');
                                        if (fieldName && self.fieldValidationMap[fieldName]) {
                                            const validationData = self.fieldValidationMap[fieldName];
                                            validationData.valMsgSpan = $(this);
                                            validationData.valMsgSpan.hide();
                                            const fieldElem = document.getElementsByName(fieldName)[0];
                                            if (fieldElem) {
                                                self.updateFieldState(fieldElem, validationData);
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    });
                });

                // Start observing the document for changes
                observer.observe(document.body, {
                    childList: true,
                    subtree: true,
                    attributes: false,
                    characterData: false
                });
            }
        },

        /**
         * Check if an element is a form field we care about
         */
        isFormField: function (element) {
            return $(element).is('input[data-val=true], textarea[data-val=true]');
        },

        /**
         * Process all form fields with validation attributes
         */
        processFields: function () {
            const self = this;
            $('input[data-val=true], textarea[data-val=true]').each(function () {
                self.processField(this);
            });
        },

        /**
         * Process a single field - extract validation metadata and setup listeners
         */
        processField: function (element) {
            const $element = $(element);
            const fieldName = element.name;

            // Skip if already processed
            if (this.fieldValidationMap[fieldName]) {
                // Even if processed, ensure we attach valMsgSpan if present now
                const existing = this.fieldValidationMap[fieldName];
                const $valMsgSpan = $('[data-valmsg-for="' + fieldName + '"]').first();
                if ($valMsgSpan && $valMsgSpan.length) {
                    existing.valMsgSpan = $valMsgSpan;
                    // hide initially to avoid duplicate messages until state updated
                    existing.valMsgSpan.hide();
                    this.updateFieldState(element, existing);
                }
                return;
            }

            // Extract validation metadata
            const validationData = this.extractValidationMetadata(element);

            // Find existing unobtrusive validation message span for this field (if any)
            const $valMsgSpan = $('[data-valmsg-for="' + fieldName + '"]').first();
            if ($valMsgSpan && $valMsgSpan.length) {
                validationData.valMsgSpan = $valMsgSpan; // store jQuery object for later show/hide
                // hide initially to avoid duplicate messages; will be toggled in updateFieldState
                validationData.valMsgSpan.hide();
            }

            // Store the metadata
            this.fieldValidationMap[fieldName] = validationData;

            // If this field has length validation, setup UI elements
            if (validationData.hasLengthValidation) {
                this.setupFieldUI(element, validationData);

                // Apply maxlength attribute if MaximumLength is defined
                if (validationData.maxLength) {
                    element.maxLength = validationData.maxLength;
                }

                // Initial state update
                this.updateFieldState(element, validationData);
            }
        },

        /**
         * Extract validation metadata from data-val-* attributes
         * This reads FluentValidation unobtrusive attributes
         */
        extractValidationMetadata: function (element) {
            const $element = $(element);
            const metadata = {
                minLength: null,
                maxLength: null,
                minLengthMessage: null,
                maxLengthMessage: null,
                hasLengthValidation: false,
                valMsgSpan: null
            };

            // Check for data-val-length attribute (both min and max)
            if ($element.attr('data-val-length')) {
                metadata.maxLength = parseInt($element.attr('data-val-length-max')) || null;
                metadata.minLength = parseInt($element.attr('data-val-length-min')) || null;
                metadata.maxLengthMessage = $element.attr('data-val-length') || 'Maximum length exceeded';
                metadata.minLengthMessage = $element.attr('data-val-length') || 'Minimum length not met';
                metadata.hasLengthValidation = true;
            }

            // Check for data-val-maxlength attribute (FluentValidation MaximumLength rule)
            if ($element.attr('data-val-maxlength')) {
                metadata.maxLength = parseInt($element.attr('data-val-maxlength-max')) || metadata.maxLength || null;
                metadata.maxLengthMessage = $element.attr('data-val-maxlength') || metadata.maxLengthMessage || 'Maximum length exceeded';
                metadata.hasLengthValidation = true;
            }

            // Check for data-val-minlength attribute (FluentValidation MinimumLength rule)
            if ($element.attr('data-val-minlength')) {
                metadata.minLength = parseInt($element.attr('data-val-minlength-min')) || metadata.minLength || null;
                metadata.minLengthMessage = $element.attr('data-val-minlength') || metadata.minLengthMessage || 'Minimum length not met';
                metadata.hasLengthValidation = true;
            }

            return metadata;
        },

        /**
         * Setup UI elements for a field (min-length and max-length message containers only)
         */
        setupFieldUI: function (element, validationData) {
            const $element = $(element);
            const fieldName = element.name;

            // Remove any leftover counter element adjacent to field
            $element.next('small.char-counter').remove();

            // Add min-length message container if minLength exists
            if (validationData.minLength) {
                // If there's an unobtrusive valMsg span, we will prefer not to duplicate. We'll create a small only if no valMsg span.
                if (!validationData.valMsgSpan) {
                    const $minLengthMsg = $(this.config.minLengthMessageTemplate);
                    $minLengthMsg.attr('data-field', fieldName);
                    $minLengthMsg.text(validationData.minLengthMessage);
                    $element.after($minLengthMsg);
                } else {
                    // Store message for potential use and ensure span initially hidden
                    validationData.valMsgSpan.data('fluent-min-msg', validationData.minLengthMessage);
                    validationData.valMsgSpan.hide();
                }
            }

            // Add max-length message container if maxLength exists
            if (validationData.maxLength) {
                if (!validationData.valMsgSpan) {
                    const $maxLengthMsg = $(this.config.maxLengthMessageTemplate);
                    $maxLengthMsg.attr('data-field', fieldName);
                    $maxLengthMsg.text(validationData.maxLengthMessage);
                    $element.after($maxLengthMsg);
                } else {
                    // Save message on existing span and keep it hidden until needed
                    validationData.valMsgSpan.data('fluent-max-msg', validationData.maxLengthMessage);
                    validationData.valMsgSpan.hide();
                }
            }
        },

        /**
         * Handle input event for a field
         */
        handleFieldInput: function (element) {
            const fieldName = element.name;
            const validationData = this.fieldValidationMap[fieldName];

            // If we haven't processed this field yet, do it now
            if (!validationData) {
                this.processField(element);
                return;
            }

            // Update field state (messages, etc.)
            this.updateFieldState(element, validationData);
        },

        /**
         * Update the visual state of a field based on current input
         */
        updateFieldState: function (element, validationData) {
            const $element = $(element);
            const fieldName = element.name;
            const currentLength = (element.value || '').length;

            // Helper to find our small messages if they exist
            const $minSmall = $(`small.min-length-message[data-field="${fieldName}"]`);
            const $maxSmall = $(`small.max-length-message[data-field="${fieldName}"]`);
            const $valMsgSpan = validationData && validationData.valMsgSpan ? validationData.valMsgSpan : null;

            // Handle minimum length validation
            if (validationData.minLength) {
                if ($minSmall.length) {
                    if (currentLength < validationData.minLength && currentLength > 0) {
                        $minSmall.show();
                        if ($valMsgSpan) $valMsgSpan.hide();
                    } else {
                        $minSmall.hide();
                        if ($valMsgSpan) $valMsgSpan.show();
                    }
                } else if ($valMsgSpan) {
                    if (currentLength < validationData.minLength && currentLength > 0) {
                        $valMsgSpan.text(validationData.minLengthMessage).show();
                    } else {
                        $valMsgSpan.text('').hide();
                    }
                }
            }

            // Handle maximum length validation
            if (validationData.maxLength) {
                if ($maxSmall.length) {
                    if (currentLength >= validationData.maxLength) {
                        // At or exceeding max length
                        $maxSmall.show();

                        // Prevent further input by truncating
                        if (currentLength > validationData.maxLength) {
                            element.value = element.value.substring(0, validationData.maxLength);
                        }

                        // Mark field as having an error
                        $element.addClass(this.config.errorClass);

                        // Hide unobtrusive span to avoid duplicate messages
                        if ($valMsgSpan) $valMsgSpan.hide();
                    } else {
                        // Below max length
                        $maxSmall.hide();
                        $element.removeClass(this.config.errorClass);

                        // Restore unobtrusive span
                        if ($valMsgSpan) $valMsgSpan.show();
                    }
                } else if ($valMsgSpan) {
                    if (currentLength >= validationData.maxLength) {
                        // Show custom max message in the span
                        $valMsgSpan.text(validationData.maxLengthMessage).show();
                        $element.addClass(this.config.errorClass);
                        // Truncate input
                        if (currentLength > validationData.maxLength) {
                            element.value = element.value.substring(0, validationData.maxLength);
                        }
                    } else {
                        // Clear custom message and hide span so validation plugin can manage it
                        $valMsgSpan.text('').hide();
                        $element.removeClass(this.config.errorClass);
                    }
                }
            }
        },

        /**
         * Reprocess all fields (useful after dynamic content is added)
         */
        reprocess: function () {
            this.processFields();
        },

        /**
         * Get validation data for a specific field
         */
        getFieldValidationData: function (fieldName) {
            return this.fieldValidationMap[fieldName] || null;
        },

        /**
         * Reset validation state for a field
         */
        resetField: function (fieldName) {
            const element = $(`[name="${fieldName}"]`)[0];
            if (element && this.fieldValidationMap[fieldName]) {
                element.value = '';
                this.updateFieldState(element, this.fieldValidationMap[fieldName]);

                // Hide messages
                $(`small.min-length-message[data-field="${fieldName}"]`).hide();
                $(`small.max-length-message[data-field="${fieldName}"]`).hide();

                // Also clear and hide valMsgSpan if present
                const vd = this.fieldValidationMap[fieldName];
                if (vd && vd.valMsgSpan) {
                    vd.valMsgSpan.text('').hide();
                }
            }
        },

        /**
         * Destroy validator instance (cleanup)
         */
        destroy: function () {
            this.fieldValidationMap = {};
            // Remove any leftover counters and messages
            $('small.char-counter, small.min-length-message, small.max-length-message').remove();
        }
    };

    // Make it globally available
    window.FluentValidationLengthValidator = FluentValidationLengthValidator;

    // Auto-initialize on document ready
    $(document).ready(function () {
        FluentValidationLengthValidator.init();
    });

})(window);
