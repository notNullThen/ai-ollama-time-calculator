/**
 * Copies the inner text of an element to the clipboard and provides visual feedback on a button.
 * @param {string} elementId - The ID of the element containing the text to copy.
 * @param {HTMLElement} btn - The button element that was clicked.
 */
window.copyToClipboard = function (elementId, btn) {
    const element = document.getElementById(elementId);
    if (!element) return;

    const text = element.innerText;
    const originalText = btn.innerText;

    const showFeedback = () => {
        btn.innerText = 'Copied!';
        btn.classList.add('btn-success');
        btn.classList.remove('btn-outline-primary');

        setTimeout(() => {
            btn.innerText = originalText;
            btn.classList.remove('btn-success');
            btn.classList.add('btn-outline-primary');
        }, 2000);
    };

    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text)
            .then(showFeedback)
            .catch(err => {
                console.error('Failed to copy text: ', err);
            });
    } else {
        // Fallback for older browsers or non-secure contexts
        const textarea = document.createElement('textarea');
        textarea.value = text;
        // Ensure textarea is not visible but part of the DOM
        textarea.style.position = 'fixed';
        textarea.style.left = '-9999px';
        textarea.style.top = '0';
        document.body.appendChild(textarea);
        textarea.select();
        try {
            document.execCommand('copy');
            showFeedback();
        } catch (err) {
            console.error('Fallback copy failed: ', err);
        }
        document.body.removeChild(textarea);
    }
};

/**
 * Sets up auto-scrolling for a container, disabling it on manual scroll and enabling via button.
 */
window.setupAutoScroll = function(containerId, buttonId) {
    const container = document.getElementById(containerId);
    const button = document.getElementById(buttonId);
    if (!container || !button) return;

    // Prevent attaching multiple observers
    if (container.dataset.autoScrollInitialized) return;
    container.dataset.autoScrollInitialized = "true";

    let isAutoScrollEnabled = true;

    const updateButtonState = () => {
        if (isAutoScrollEnabled) {
            button.classList.add('btn-primary');
            button.classList.remove('btn-outline-primary');
            button.innerHTML = 'Auto-scroll: ON';
        } else {
            button.classList.remove('btn-primary');
            button.classList.add('btn-outline-primary');
            button.innerHTML = 'Auto-scroll: OFF';
        }
    };

    button.addEventListener('click', () => {
        isAutoScrollEnabled = true;
        container.scrollTop = container.scrollHeight;
        updateButtonState();
    });

    let lastScrollTop = container.scrollTop;

    container.addEventListener('scroll', () => {
        // Only disable if user scrolled UP
        if (container.scrollTop < lastScrollTop) {
            const isAtBottom = container.scrollHeight - container.scrollTop <= container.clientHeight + 10;
            if (!isAtBottom && isAutoScrollEnabled) {
                isAutoScrollEnabled = false;
                updateButtonState();
            }
        }
        lastScrollTop = container.scrollTop;
    });

    // Observer to scroll to bottom when content changes
    const observer = new MutationObserver(() => {
        if (isAutoScrollEnabled) {
            container.scrollTop = container.scrollHeight;
            lastScrollTop = container.scrollTop;
        }
    });
    observer.observe(container, { childList: true, subtree: true, characterData: true });

    // Initial state
    updateButtonState();
    if (isAutoScrollEnabled) {
        container.scrollTop = container.scrollHeight;
        lastScrollTop = container.scrollTop;
    }
};
