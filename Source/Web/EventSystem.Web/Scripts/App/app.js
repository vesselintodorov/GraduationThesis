

var app = function () {

    $(document).ready(function () {
        $('.datepicker').datepicker({
            orientation: "bottom auto"
        }); //Initialise any date pickers

        $(".browseFilter").change(onBrowseFilterControlChange);

    });

    function onBrowseFilterControlChange() {
        $("#browseForm").submit();
    }
}();