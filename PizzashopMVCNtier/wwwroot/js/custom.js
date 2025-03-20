function toggleCheckBoxes(source) {
    let checkBoxs = document.querySelectorAll(".row-checkbox");
  
    checkBoxs.forEach((checkbox) => (checkbox.checked = source.checked));
  }
  
  // Loading Spinner
$(document).on("submit", "form", function (e) {
    if (!$(this).valid()) {
      e.preventDefault();
    }
  });
  
  // Loading Spinner
  $(document).ready(function () {
    $("#loader").hide();
  
    // Loader on Ajax
    // $(document).ajaxStart(function () {
    //   $("#loader").show();
    // });
  
    // $(document).ajaxStop(function () {
    //   $("#loader").hide();
    // });
  
    $(document).on("submit", "form", function (e) {
        $("#loader").show();
    });
  
  
    $(window).on("load", function () {
        $("#loader").hide();
    });
  });