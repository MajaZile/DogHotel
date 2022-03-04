var onResize = function() {
    // apply dynamic padding at the top of the body according to the fixed navbar height
    $("body").css("padding-top", $(".navbar-fixed-top").height());
  //  $(".wrapper").css("min-height", `calc(100% - (${$("footer").height()}px + ${$(".navbar-fixed-top").height()}px))`);
   // $(".content").css("min-height", `calc(100% - (${$("footer").height()}px + ${$(".navbar-fixed-top").height()}px))`);
   $(".new-usluga").css("height", $(".pricing-table .item").height());
  };
  
  // attach the function to the window resize event
  $(window).resize(onResize);
  
  // call it also when the page is ready after load or reload
  $(function() {
    onResize();
  });

  // caroseul
  $('.carousel').carousel({
    interval: 3000
})

function OnLozinkaChange() {
  var element = document.getElementById("confirmpass");
  element.removeClass("hidden");
}

$('#Lozinka2').change(function () {
  alert('Changed!')
  var element = document.getElementById("confirmpass");
  element.removeClass("hidden");
});