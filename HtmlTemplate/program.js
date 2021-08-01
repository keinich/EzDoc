var toggler = document.getElementsByClassName("caret");
var contentLinks = document.getElementsByClassName("content-link");
var contentLinksInline = document.getElementsByClassName("content-link-inline");
var currentActiveElementId = ""
var currentActiveLinkId = ""
var i;


for (i = 0; i < toggler.length; i++) {
  toggler[i].addEventListener("click", function () {
    this.parentElement.querySelector(".nested").classList.toggle("active");
    this.classList.toggle("caret-down");
  });
}

for (i = 0; i < contentLinks.length; i++) {
  contentLinks[i].addEventListener("click", function () {
    if (currentActiveElementId !== "") {
      document.getElementById(currentActiveElementId).classList.remove("active-content");
    }
    if (currentActiveLinkId !== "") {
      document.getElementById(currentActiveLinkId).classList.remove("active-content-link");
    }
    currentActiveElementId = this.innerHTML;
    currentActiveLinkId = currentActiveElementId + "-link";
    document.getElementById(currentActiveElementId).classList.add("active-content");
    this.classList.add("active-content-link");
  });
}

for (i = 0; i < contentLinksInline.length; i++) {
  contentLinksInline[i].addEventListener("click", function () {
    if (currentActiveElementId !== "") {
      document.getElementById(currentActiveElementId).classList.remove("active-content");
    }
    if (currentActiveLinkId !== "") {
      document.getElementById(currentActiveLinkId).classList.remove("active-content-link");
    }
    currentActiveElementId = this.innerHTML;
    currentActiveLinkId = currentActiveElementId + "-link";
    document.getElementById(currentActiveElementId).classList.add("active-content");

    var linkElement = document.getElementById(currentActiveLinkId);
    linkElement.classList.add("active-content-link");

    openParents(linkElement);
  });
}

function openParents(linkElement) {

  var parent = linkElement.parentElement;
  if (!parent) {
    return;
  }
  var parentIsOpen = parent.classList.contains("active");
  if (parentIsOpen) {
    return;
  }
  parent.classList.add("active");

  if (parent.parentElement) {
    var carets = parent.parentElement.children;
    for (caret of carets) {
      if (caret.classList.contains("caret")) {
        caret.classList.add("caret-down");
      }
    }
  }

  openParents(parent);
}