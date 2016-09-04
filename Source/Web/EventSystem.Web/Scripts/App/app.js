

var app = function () {

    $(document).ready(function () {

        fixJqueryValidationForChrome();
        manageCollapsableDivs();
        getNotificationsDdlData();
        $("#addEventContainer #Type").change(onAddEventTypeChange);
        $('.datepicker').datetimepicker({ format: 'dd/mm/yyyy hh:ii', language: 'bg' });
        $(".browseFilter").change(onBrowseFilterControlChange);
        manageGridEvents();
        $("#subscribeBtn").click(onEventSubscribe);
        $("#unsubscribeBtn").click(onEventUnsubscribe);
        bindExternalLectureOpen();
        $(document).on("click", ".btnExpellUser", onExpellUser);
        $("#SearchedEventName").focusout(onSearchEventBoxFocusOut);
        $(".notificationsBtn").click(onNotificationsBtnClick);

        if ($("#commentsDiv")) {
            loadEventComments();
        }

        if ($(".pagination-container")) {
            $(".pagination-container").addClass("col-md-12");
            $(".onPagingButtonClick .pagination a").click(onPagingButtonClick)
        }

        //if ($("#addCommentBtn")) {
        //    $("#addCommentBtn").click(onAddCommentClick);
        //}
    });

    function onNotificationsBtnClick() {
        sessionStorage.setItem("IsUserNotified", "true");
        sessionStorage.setItem("NotificationsCount", "0");
        $(".badge-notify").addClass("badge-hidden");
        //$(".badge-notify").text("");

    }

    function bindExternalLectureOpen() {
        if ($("#ExternallySelectedLectureId").val() > 0) {
            $.ajax({
                url: "/Event/DisplayLecture",
                type: "POST",
                data: { lectureId: $("#ExternallySelectedLectureId").val() },
                success: function (data) {
                    debugger;
                    $("#lectureModalContainer").html(data);
                    $('#lectureModal').modal('show');
                }
            });
        }
    }

    function fixJqueryValidationForChrome() {
        if (jQuery.validator) {
            jQuery.validator.methods.date = function (value, element) {
                var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
                if (isChrome) {
                    var d = new Date();
                    return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value)));
                } else {
                    return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
                }
            };
        }
        if ($.validator) {

            $.validator.addMethod(
            "date",
            function (value, element) {
                var bits = value.match(/([0-9]+)/gi), str;
                if (!bits)
                    return this.optional(element) || false;
                str = bits[1] + '/' + bits[0] + '/' + bits[2];
                return this.optional(element) || !/Invalid|NaN/.test(new Date(str));
            },
            "Please enter a date in the format dd/mm/yyyy"

            );
        }
    }

    function getNotificationsDdlData() {
        debugger;
        $.ajax({
            url: "/Common/GetNotificationsData",
            type: "POST",
            data: {},
            success: function (data) {
                debugger;
                $("#notifications li:not(:first)").remove();
                if (data.length > 0) {
                    debugger;

                    if (sessionStorage.getItem("IsUserNotified") != "true") {
                        $(".badge-notify").removeClass("badge-hidden");
                        if (parseInt(sessionStorage.getItem("NotificationsCount")) > 0) {
                            $(".badge-notify").text(sessionStorage.getItem("NotificationsCount"));
                        }
                        else {
                            $(".badge-notify").text(data.length);
                        }
                    }
                    else {
                        $(".badge-notify").addClass("badge-hidden");
                        //$(".badge-notify").text("");
                    }

                    $("li#noNotifications").hide();
                    $(data).each(function () {
                        debugger;

                        var currentItem = "<li><a href='/Event/Display/?eventId=" + this.Id + "&lectureId=" + this.LectureId
                            + "'><i " + (this.LectureId == 0 ? "class='fa fa-calendar'" : "class='fa fa-calendar-o'")
                            + ' aria-hidden="true"></i> ' + this.Title + "<div class='notificationSecondaryText text-danger'>след "
                            + (this.HoursRemaining > 0 ? +this.HoursRemaining + (this.HoursRemaining == 1 ? " час" : " часа") + " и " : "") + this.MinutesRemaining
                            + " минути</div><div class='notificationSecondaryText'><strong>" + this.TypeMessage + "</strong></div></a></li>"

                        //<i class="fa fa-calendar-o" aria-hidden="true"></i>
                        //if (this.HoursRemaining > 0) {
                        //    $("#notifications").append("<li><a href='/Event/Display/?eventId=" + this.Id + "&lectureId=" + this.LectureId + "'>" + this.Title + "<div class='notificationTimeRemaining text-danger'>след " +  this.HoursRemaining + " часа и " + this.MinutesRemaining + " минути</div></a></li>")
                        //} else {
                        //    $("#notifications").append("<li><a href='/Event/Display/?eventId=" + this.Id + "&lectureId=" + this.LectureId + "'>" + this.Title + "<div class='notificationTimeRemaining text-danger'>след " +  this.MinutesRemaining + " минути</div></a></li>")
                        //}
                        $("#notifications").append(currentItem);
                    });
                } else {
                    debugger;
                    $(".badge-notify").addClass("badge-hidden");
                    $("li#noNotifications").show();
                }
                //$($.parseJSON(data)).map(function () {
                //    return $("<li><a href='/Event/Display/?eventId=" + this.Id + "'>" + this.Title + "</a></li>");
                //}).appendTo('#notifications');
            }
        });
    }

    function onAddEventTypeChange() {
        debugger;
        $.ajax({
            url: "/Event/AddEventDatePicker",
            type: "POST",
            data: { eventTypeId: $(this).val() },
            success: function (data) {
                $("#eventTypeContainer").html(data);
            }
        });
    }

    function manageGridEvents() {
        if (pageGrids.browseEventsGrid) {
            pageGrids.browseEventsGrid.onRowSelect(function (e) {
                var url = '/Event/Display/?eventId=' + e.row.Id;
                window.location.href = url;
            });
        }

        if (pageGrids.userEventsGrid) {
            pageGrids.userEventsGrid.onRowSelect(function (e) {
                var url = '/Event/Display/?eventId=' + e.row.Id;
                window.location.href = url;
            });
        }
    }

    function manageCollapsableDivs() {
        $('.collapse').on('shown.bs.collapse', function () {
            $(this).parent().find(".glyphicon-chevron-down").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
        }).on('hidden.bs.collapse', function () {
            $(this).parent().find(".glyphicon-chevron-up").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
        });
    }

    function onBrowseFilterControlChange() {
        $("#browseForm").submit();
    }



    function onEventSubscribe() {
        debugger;
        $.ajax({
            url: "/Event/SubscribeUser",
            type: "POST",
            data: { eventId: $("#EventId").val() },
            success: function (data) {
                debugger;
                $("#subscribeBtn").hide();
                $("#unsubscribeBtn").show();

                sessionStorage.setItem("IsUserNotified", "false");
                var notificationsCount = parseInt(sessionStorage.getItem("NotificationsCount")) + 1;
                sessionStorage.setItem("NotificationsCount", notificationsCount);
                getNotificationsDdlData();

                var alertMessageElement = "<div class='alert alert-" + data.alertType + "'><strong>" + data.alertMsg + "</strong></div>";
                $("#eventMessageContainer").append(alertMessageElement);
            },
            done: new function () {
                setTimeout(function () {
                    $($("#eventMessageContainer .alert")).fadeOut(200);

                }, 3000);
            }
        });
    }

    function onEventUnsubscribe() {
        $.ajax({
            url: "/Event/UnsubscribeUser",
            type: "POST",
            data: { eventId: $("#EventId").val() },
            success: function (data) {
                debugger;
                $("#unsubscribeBtn").hide();
                $("#subscribeBtn").show();

                sessionStorage.setItem("IsUserNotified", "true");
                var notificationsCount = 0;
                if (parseInt(sessionStorage.getItem("NotificationsCount") > 0)) {
                    notificationsCount = parseInt(sessionStorage.setItem("NotificationsCount")) - 1;
                }
                sessionStorage.setItem("NotificationsCount", notificationsCount);
                getNotificationsDdlData();

                var alertMessageElement = "<div class='alert alert-" + data.alertType + "'><strong>" + data.alertMsg + "</strong></div>";
                $("#eventMessageContainer").append(alertMessageElement);

            },
            done: new function () {
                setTimeout(function () {
                    $($("#eventMessageContainer .alert")).fadeOut(200);

                }, 3000);
            }

        });
    }

    function onExpellUser() {
        $.ajax({
            url: "/Event/ExpellUser",
            type: "POST",
            data: {
                eventId: $("#EventId").val(),
                eventUserId: $(this).closest("tr").find(".eventUserId").text(),
            },
            success: function (data) {
                refreshUsersGridWithAlert(data.alertType, data.alertMsg);
            }
        });
    }

    function refreshUsersGridWithAlert(alertType, alertMsg) {
        $.ajax({
            url: "/Event/UsersGrid",
            type: "POST",
            data: { eventId: $("#EventId").val() },
            success: function (data) {

                $("#usersGridContainer").html(data);
                var alertMessageElement = "<div class='alert alert-" + alertType + "'><strong>" + alertMsg + "</strong></div>";
                $("#usersGridContainer").append(alertMessageElement);
                setTimeout(function () {
                    $("#usersGridContainer .alert").fadeOut(200);
                }, 3000);
            }
        });
    }

    function onSearchEventBoxFocusOut() {
        $("#browseForm").submit();
    }

    function loadEventComments() {

        var currentEventId = $("#EventId").val();
        if (currentEventId) {
            $.ajax({
                url: "/Event/Comments",
                type: "POST",
                data: { eventId: currentEventId },
                success: function (data) {
                    $("#commentsDiv").html(data);
                }
            });
        }

    }

    function onPagingButtonClick(e) {
        e.preventDefault();
        alert("yea");
        //$(".onPagingButtonClick .pagination a")
    }

    //function onAddCommentClick() {
    //    $.ajax({
    //        url: "/Event/AddComment",
    //        type: "POST",
    //        data: { eventId: $("#EventId").val(), title: $("#commentTitle").val(), content: $("#commentContent").val() },
    //        success: function (data) {

    //        }
    //    });
    //}

    function clearTextFieldsInForm(formId) {
        $('#' + formId).find("input[type='text'], input[type='password'], textarea").val("");
    }


    return {
        loadEventComments: loadEventComments,
        clearTextFieldsInForm: clearTextFieldsInForm
    }

}();