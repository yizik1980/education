angular.module('boardingSchoolApp').service('ToastService', ['$timeout', function($timeout) {
    const self = this;
    this.toasts = [];

    this.show = function(message, type) {
        const toast = { id: Date.now(), message, type: type || 'success', visible: true };
        self.toasts.push(toast);
        $timeout(function() {
            toast.visible = false;
            $timeout(function() {
                const idx = self.toasts.indexOf(toast);
                if (idx !== -1) self.toasts.splice(idx, 1);
            }, 400);
        }, 4000);
    };

    this.dismiss = function(toast) {
        toast.visible = false;
        $timeout(function() {
            const idx = self.toasts.indexOf(toast);
            if (idx !== -1) self.toasts.splice(idx, 1);
        }, 400);
    };
}]);
