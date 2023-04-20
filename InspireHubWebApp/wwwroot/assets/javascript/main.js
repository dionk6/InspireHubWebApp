function openNavLinks() {
  var element = document.getElementById("top-header");

  if (element.classList.contains("active")) {
    element.classList.remove("active");
  } else {
    element.classList.add("active");
  }
}
