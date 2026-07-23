// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Only run notification logic if the notification element exists in the DOM
    const $dropdownContainer = $('#notificationDropdownContainer');
    if ($dropdownContainer.length === 0) return;

    const $badge = $('#notificationBadge');
    const $bellIcon = $('#notificationBellIcon');
    const $list = $('#notificationList');
    const $emptyState = $('#notificationEmptyState');
    const $markAllAsReadBtn = $('#markAllAsReadBtn');

    // 1. Load Initial Unread Count
    function loadUnreadCount() {
        $.get('/Notifications/UnreadCount')
            .done(function (data) {
                updateBadge(data.count);
            })
            .fail(function (err) {
                console.error("Failed to load notifications count", err);
            });
    }

    // 2. Update Badge UI
    function updateBadge(count) {
        if (count > 0) {
            $badge.text(count).removeClass('d-none').addClass('pulse-badge');
            $bellIcon.addClass('has-unread').removeClass('text-muted');
        } else {
            $badge.addClass('d-none').removeClass('pulse-badge').text('0');
            $bellIcon.removeClass('has-unread').addClass('text-muted');
        }
    }

    // Helper to format date cleanly
    function formatTimeAgo(dateString) {
        // If the dateString doesn't specify a timezone, append 'Z' to parse it as UTC
        let parsedDateString = dateString;
        if (dateString && !dateString.endsWith('Z') && !dateString.includes('+') && !dateString.includes('-')) {
            parsedDateString = dateString + 'Z';
        } else if (dateString && dateString.includes('-') && dateString.split('-').length === 3 && !dateString.includes('T')) {
            // Standard date-only format, skip appending 'Z'
        } else if (dateString && !dateString.endsWith('Z') && dateString.includes('T') && !dateString.includes('+') && dateString.lastIndexOf('-') < dateString.indexOf('T')) {
            // Contains 'T' and '-' but only as date separator (no timezone offset)
            parsedDateString = dateString + 'Z';
        }

        const date = new Date(parsedDateString);
        const now = new Date();
        const diffMs = now - date;
        const diffMins = Math.floor(diffMs / 60000);
        if (diffMins < 1) return 'Just now';
        if (diffMins < 60) return `${diffMins}m ago`;
        const diffHrs = Math.floor(diffMins / 60);
        if (diffHrs < 24) return `${diffHrs}h ago`;
        const diffDays = Math.floor(diffHrs / 24);
        if (diffDays === 1) return 'Yesterday';
        if (diffDays < 7) return `${diffDays}d ago`;
        return date.toLocaleDateString();
    }

    // Helper to get type-specific icons
    function getNotificationIconClass(type) {
        // Enums: ApplicationSubmitted = 0, ApplicationAccepted = 1, ApplicationRejected = 2
        switch (type) {
            case 0: // ApplicationSubmitted
                return { icon: 'bi-file-earmark-plus-fill', styleClass: 'submitted' };
            case 1: // ApplicationAccepted
                return { icon: 'bi-check-circle-fill', styleClass: 'accepted' };
            case 2: // ApplicationRejected
                return { icon: 'bi-x-circle-fill', styleClass: 'rejected' };
            default:
                return { icon: 'bi-bell-fill', styleClass: 'default' };
        }
    }

    // Helper to get Redirect URL
    function getNotificationRedirectUrl(notif) {
        if (notif.type === 0) {
            return `/Application/ForJob?jobId=${notif.relatedEntityId}`;
        } else {
            return `/Application/Details/${notif.relatedEntityId}`;
        }
    }

    // Render a single notification item
    function createNotificationHtml(notif) {
        const iconConfig = getNotificationIconClass(notif.type);
        const url = getNotificationRedirectUrl(notif);
        const unreadClass = notif.isRead ? '' : 'unread';
        const timeAgo = formatTimeAgo(notif.createdDate);

        return `
            <a href="${url}" class="notification-item ${unreadClass}" data-id="${notif.id}">
                <div class="notification-icon-wrapper ${iconConfig.styleClass}">
                    <i class="bi ${iconConfig.icon}"></i>
                </div>
                <div class="notification-content">
                    <div class="notification-message">${escapeHtml(notif.message)}</div>
                    <div class="notification-time">${timeAgo}</div>
                </div>
            </a>
        `;
    }

    function escapeHtml(str) {
        return str
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    // 3. Fetch Notifications and populate list
    function loadNotifications() {
        if ($list.find('.notification-item').length === 0) {
            $list.html('<div class="text-center py-4"><div class="spinner-border spinner-border-sm text-primary" role="status"></div></div>');
        }

        $.get('/Notifications')
            .done(function (notifications) {
                $list.empty();
                if (notifications && notifications.length > 0) {
                    $emptyState.addClass('d-none');
                    $markAllAsReadBtn.removeClass('d-none');
                    notifications.forEach(function (notif) {
                        $list.append(createNotificationHtml(notif));
                    });
                } else {
                    $list.append($emptyState.removeClass('d-none'));
                    $markAllAsReadBtn.addClass('d-none');
                }
            })
            .fail(function (err) {
                console.error("Failed to load notifications list", err);
                $list.html('<div class="text-center text-danger py-3" style="font-size: 0.85rem;">Error loading notifications</div>');
            });
    }

    // 4. Mark notifications as read
    function markAllAsRead() {
        $.get('/Notifications/MarkAllAsRead')
            .done(function () {
                updateBadge(0);
                $('.notification-item.unread').removeClass('unread');
                $markAllAsReadBtn.addClass('d-none');
            })
            .fail(function (err) {
                console.error("Failed to mark notifications as read", err);
            });
    }

    // 5. Connect to SignalR
    let connection = null;
    try {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveNotification", function (notification) {
            // Re-fetch unread count
            loadUnreadCount();

            // Shake the bell
            $bellIcon.removeClass('has-unread');
            setTimeout(function() {
                $bellIcon.addClass('has-unread');
            }, 50);

            // Prepend if list is visible
            if ($dropdownContainer.hasClass('show') || $('.notification-dropdown-menu').hasClass('show')) {
                $emptyState.addClass('d-none');
                $markAllAsReadBtn.removeClass('d-none');
                
                // Remove duplicates (e.g. if the notification was aggregated and updated)
                const duplicateSelector = `.notification-item[data-id="${notification.id}"]`;
                $(duplicateSelector).remove();

                $list.prepend(createNotificationHtml(notification));
            }
        });

        connection.start()
            .then(function () {
                console.log("SignalR Connected to notificationHub");
            })
            .catch(function (err) {
                console.error("SignalR Connection Error: ", err.toString());
            });
    } catch (e) {
        console.error("Could not initialize SignalR client", e);
    }

    // 6. Event Listeners
    loadUnreadCount();

    const dropdownElement = document.getElementById('notificationDropdown');
    if (dropdownElement) {
        dropdownElement.addEventListener('show.bs.dropdown', function () {
            loadNotifications();
            if ($badge.text() !== '0' && !$badge.hasClass('d-none')) {
                markAllAsRead();
            }
        });
    }

    $markAllAsReadBtn.on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        markAllAsRead();
    });
});

function toggleSidebar() {

    const sidebar = document.getElementById("sidebar");
    const content = document.getElementById("main-content");

    sidebar.classList.toggle("open");
    content.classList.toggle("expanded");
}
