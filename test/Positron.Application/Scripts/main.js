$(function () {
    $("#TestAjax").click(function (e) {
        e.preventDefault();

        $.ajax("/Positron.Application/Home/TestAjax", {
            method: "post",
            data: JSON.stringify({ value: "Success!" }),
            contentType: "application/json",
            processData: false,
            dataType: "json"
        }).then(
            function (data) {
                window.alert((data && data.value) || "No data returned");
            },
            function () {
                window.alert("Error!");
            });
    });
});