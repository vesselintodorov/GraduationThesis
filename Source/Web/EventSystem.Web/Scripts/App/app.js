

var app = function () {

    $(document).ready(function () {
        //$('.datepicker').datepicker({
        //    orientation: "bottom auto"
        //}); //Initialise any date pickers

        $('.datepicker').datetimepicker({ format: 'dd/mm/yyyy hh:ii', language: 'bg' });

        $(".browseFilter").change(onBrowseFilterControlChange);

    });

    function onBrowseFilterControlChange() {
        $("#browseForm").submit();
    }
}();