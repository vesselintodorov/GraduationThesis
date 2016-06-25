

var app = function () {

    $(document).ready(function () {

        fixJqueryValidationForChrome();
        manageCollapsableDivs();
        $('.datepicker').datetimepicker({ format: 'dd/mm/yyyy hh:ii', language: 'bg' });
        $(".browseFilter").change(onBrowseFilterControlChange);
        manageGridEvents();
        $("#subscribeBtn").click(onEventSubscribe);
        $("#unsubscribeBtn").click(onEventUnsubscribe);
        $(document).on("click", ".btnExpellUser", onExpellUser);
    });

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

    function manageGridEvents() {
        if (pageGrids.browseEventsGrid) {
            pageGrids.browseEventsGrid.onRowSelect(function (e) {
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
                var alertMessageElement = "<div class='alert alert-" + data.alertType + "'><strong>" + data.alertMsg + "</strong></div>";
                $("#eventMessageContainer").append(alertMessageElement);
            },
            done: new function () {
                setTimeout(function () {
                    $($("#eventMessageContainer .alert")).fadeOut(400);
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



    return {

    }

}();