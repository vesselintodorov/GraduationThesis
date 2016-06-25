

var lectures = function () {
    $(document).ready(function () {
        $(document).on("click", ".btnDeleteLecture", onDeleteLecture);
        $(document).on("click", "#lecturesGridContainer .grid-mvc tr", onLecturesGridClick);
    });

    function onAddLectureSucess() {
        debugger;
        showAndfadeOutElement("#addLectureContainer .alert-success");
        refreshLecturesGrid();
    }



    function onAddLectureFailure() {
        showAndfadeOutElement("#addLectureContainer .alert-danger");
    }

    function onDeleteLecture(e) {
        e.preventDefault();
        e.stopPropagation();
        debugger;
        $.ajax({
            url: "/Event/DeleteLecture",
            type: "POST",
            data: {
                eventId: $("#EventId").val(),
                lectureId: $(this).closest("tr").find(".lectureId").text(),
            },
            success: function (data) {
                refreshLecturesGridWithAlert(data.alertType, data.alertMsg);
            },

        });
    }

    function refreshLecturesGridWithAlert(alertType, alertMsg) {
        $.ajax({
            url: "/Event/LecturesGrid",
            type: "POST",
            data: { eventId: $("#EventId").val() },
            success: function (data) {
                debugger;
                $("#lecturesGridContainer").html(data);
                var alertMessageElement = "<div class='alert alert-" + alertType + "'><strong>" + alertMsg + "</strong></div>";
                $("#lecturesGridContainer").append(alertMessageElement);
                setTimeout(function () {
                    $("#lecturesGridContainer .alert").fadeOut(200);
                }, 3000);
            }
        });
    }

    function refreshLecturesGrid() {
        $.ajax({
            url: "/Event/LecturesGrid",
            type: "POST",
            data: { eventId: $("#EventId").val() },
            success: function (data) {

                $("#lecturesGridContainer").html(data);
            }
        });
    }

    function showAndfadeOutElement(element) {
        $(element).show();
        setTimeout(function () {
            $(element).fadeOut(200);
        }, 3000);
    }

    function onLecturesGridClick() {
        debugger;
        $(this).parent().find("tr").each(function () {
            $(this).removeClass("grid-row-selected");
        });
        $(this).addClass("grid-row-selected");
        $.ajax({
            url: "/Event/DisplayLecture",
            type: "POST",
            data: { lectureId: $(this).find(".lectureId").text() },
            success: function (data) {
                debugger;
                $("#lectureModalContainer").html(data);
                $('#lectureModal').modal('show');
            }
        });
    }

    return {
        onAddLectureSucess: onAddLectureSucess,
        onAddLectureFailure: onAddLectureFailure
    }
}();