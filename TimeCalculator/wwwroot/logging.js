/**
 * Global logging utilities for TimeCalculator
 */
window.tcLogger = {
    errorStyle: "color: white; background: #dc3545; padding: 2px 4px; border-radius: 3px; font-weight: bold;",
    warningStyle: "color: black; background: #ffc107; padding: 2px 4px; border-radius: 3px; font-weight: bold;",
    infoStyle: "color: white; background: #0d6efd; padding: 2px 4px; border-radius: 3px; font-weight: bold;",

    logError: function(message, detail) {
        console.error(
            "%c[Error]%c " + message,
            this.errorStyle,
            "color: inherit;",
            detail || ""
        );
    },

    logWarning: function(message) {
        console.warn(
            "%c[Warning]%c " + message,
            this.warningStyle,
            "color: inherit;"
        );
    },

    logInfo: function(message) {
        console.log(
            "%c[Info]%c " + message,
            this.infoStyle,
            "color: inherit;"
        );
    }
};

// Hook global errors
window.onerror = function (message, source, lineno, colno, error) {
    window.tcLogger.logError("Global JS Error: " + message, { source, lineno, colno, error });
};

window.onunhandledrejection = function (event) {
    window.tcLogger.logError("Unhandled Promise Rejection", event.reason);
};
