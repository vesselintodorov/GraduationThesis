

var app = function () {

    $(document).ready(function () {
        
        fixJqueryValidationForChrome();
        manageCollapsableDivs();
        $('.datepicker').datetimepicker({ format: 'dd/mm/yyyy hh:ii', language: 'bg' });
        $(".browseFilter").change(onBrowseFilterControlChange);
        manageGridEvents();
        

        $("#subscribeBtn").click(onEventSubscribe);
        $("#unsubscribeBtn").click(onEventUnsubscribe);
        $(".btnDeleteLecture").click(onDeleteLecture);
        $(".btnExpellUser").click(onDeleteLecture);
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

    function onAddLectureSucess() {
        debugger;
        showAndfadeOutElement("#addLectureContainer .alert-success");

        $.ajax({
            url: "/Event/LecturesGrid",
            type: "POST",
            data: { eventId: $("#EventId").val() },
            success: function (data) {

                $("#lecturesGridContainer").html(data);
            }
        });
    }

    function onAddLectureFailure() {
        showAndfadeOutElement("#addLectureContainer .alert-danger");
    }

    function showAndfadeOutElement(element) {
        $(element).show();
        setTimeout(function () {
            $(element).fadeOut(400);
        }, 3000);
    }

    function onEventSubscribe() {
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

    function onDeleteLecture(e) {
        $.ajax({
            url: "/Event/DeleteLecture",
            type: "POST",
            data: {
                eventId: $("#EventId").val(),
                lectureId: $(this).closest("tr").find(".lectureId").text(),
            },
            success: function (data) {
                debugger;
                var alertMessageElement = "<div class='alert alert-" + data.alertType + "'><strong>" + data.alertMsg + "</strong></div>";
                $("#lecturesGridContainer").append(alertMessageElement);

            },
            done: new function () {
                setTimeout(function () {
                    $($("#lecturesGridContainer .alert")).fadeOut(200);
                }, 3000);
            }

        });
    }

    function onExpellUserFromLecture(e) {
        $.ajax({
            url: "/Event/DeleteLecture",
            type: "POST",
            data: {
                eventId: $("#EventId").val(),
                lectureId: $(this).closest("tr").find(".lectureId").text(),
            },
            success: function (data) {
                debugger;
                var alertMessageElement = "<div class='alert alert-" + data.alertType + "'><strong>" + data.alertMsg + "</strong></div>";
                $("#lecturesGridContainer").append(alertMessageElement);

            },
            done: new function () {
                setTimeout(function () {
                    $($("#lecturesGridContainer .alert")).fadeOut(200);
                }, 3000);
            }

        });
    }

    return {
        onAddLectureSucess: onAddLectureSucess,
        onAddLectureFailure: onAddLectureFailure
    }

}();