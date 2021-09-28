var toggler = document.getElementsByClassName("caret");
var contentLinks = document.getElementsByClassName("content-link");
var contentLinksInline = document.getElementsByClassName("content-link-inline");
var filterBox = document.getElementById("filter-box");
var currentActiveContentId = ""
var currentActiveLinkId = ""
var i;

window.onhashchange = function () {
  console.log("On Window Change", location.hash);
  onContentLinkClicked(location.hash.replace("#", ""));
}

window.onload = function () {
  console.log("On Window Load", location.hash);
  onContentLinkClicked(location.hash.replace("#", ""));
}

// Register event listeners
for (i = 0; i < toggler.length; i++) {
  toggler[i].addEventListener("click", function () {
    this.parentElement.querySelector(".nested").classList.toggle("active");
    this.classList.toggle("caret-down");
  });
}

for (i = 0; i < contentLinks.length; i++) {
  contentLinks[i].addEventListener("click", function () {
    var contentId = getContentIdFromContentLink(this);
    onContentLinkClicked(contentId);
  });
}

for (i = 0; i < contentLinksInline.length; i++) {
  contentLinksInline[i].addEventListener("click", function () {
    var contentId = getContentIdFromContentLinkInline(this);
    onContentLinkClicked(contentId);
  });
}

filterBox.addEventListener("input", function () {
  filterTree(filterBox.value);
});

// internal functions

function filterTree(text) {

  if (text == "") {
    for (i = 0; i < contentLinks.length; ++i) {
      var contentLink = contentLinks[i];
      contentLink.classList.remove("filtered-out");
    }
    for (i = 0; i < toggler.length; ++i) {
      var caret = toggler[i];
      var li = caret.parentElement;
      li.classList.remove("filtered-out");
    }
    return;
  }

  // Filter Content-Links  
  for (i = 0; i < contentLinks.length; ++i) {
    var contentLink = contentLinks[i];
    if (contentLink.innerHTML.includes(text)) {
      contentLink.classList.remove("filtered-out");
      openParents(contentLink);
    } else {
      contentLink.classList.add("filtered-out");
    }
  }

  // Unfilter carets that have visible children
  for (i = 0; i < toggler.length; ++i) {
    var caret = toggler[i];
    var li = caret.parentElement;
    var childrenLis = li.getElementsByTagName("li");
    var visible = false;
    loop1:
    for (j = 0; j < childrenLis.length; ++j) {
      var childLi = childrenLis[j];
      loop2:
      if (!childLi.classList.contains("filtered-out")) {
        li.classList.remove("filtered-out");
        visible = true;
        break loop2;
      }
    }
    if (!visible) {
      li.classList.add("filtered-out");
    }
  }
}

function getContentIdFromContentLink(contentLinkElement) {
  var contentLinkId = contentLinkElement.id;
  return contentLinkId.substr(0, contentLinkId.indexOf('-link'));
}

function getContentIdFromContentLinkInline(contentLinkElement) {
  var contentLinkId = contentLinkElement.id;
  return contentLinkId.substr(0, contentLinkId.indexOf('-link-inline'));
}

function onContentLinkClicked(contentId) {
  if (currentActiveContentId !== "") {
    var activeContentElement = document.getElementById(currentActiveContentId);
    if (activeContentElement) {
      document.getElementById(currentActiveContentId).classList.remove("active-content");
    }
  }
  if (currentActiveLinkId !== "") {
    var activeLinkElement = document.getElementById(currentActiveLinkId);
    if (activeLinkElement) {
      document.getElementById(currentActiveLinkId).classList.remove("active-content-link");
    }
  }
  currentActiveContentId = contentId;
  currentActiveLinkId = contentId + "-link";
  var contentElement = document.getElementById(contentId);
  if (contentElement && !contentElement.classList.contains("active-content")) {
    document.getElementById(contentId).classList.add("active-content");
  }
  var linkElement = document.getElementById(currentActiveLinkId);
  if (linkElement && !linkElement.classList.contains("active-content-link")) {
    linkElement.classList.add("active-content-link");
  }
  openParents(linkElement);
}

function openParents(linkElement) {

  var parents = tryGetParentLinkElement(linkElement, parentUl, parentLi, parentCaret);
  if (parents.length == 0) {
    return;
  }
  var parentUl = parents[0];
  var parentLi = parents[1];
  var parentCaret = parents[2];

  if (!parentUl.classList.contains("active")) {
    parentUl.classList.add("active");
  }
  if (!parentCaret.classList.contains("caret-down")) {
    parentCaret.classList.add("caret-down");
  }

  openParents(parentLi);
}

function tryGetParentLinkElement(linkElement, parentUl, parentLi, parentCaret) {
  var parent = linkElement.parentElement;
  if (!parent) {
    return null;
  }
  var parentParent = parent.parentElement;
  if (!parentParent) {
    return null;
  }
  var carets = parentParent.children;
  for (i = 0; i < carets.length; ++i) {
    if (carets[i].classList.contains("caret"))
      return [parent, parentParent, carets[i]];
  }

  return [];
}