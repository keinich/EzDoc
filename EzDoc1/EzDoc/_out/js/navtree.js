currentActiveContentId = "";
filterBox = null;
treeView = null;

function onNodeClick(e) {
  var targetElementOld = document.getElementById(currentActiveContentId);
  if (targetElementOld !== null && targetElementOld !== undefined) {
    targetElementOld.classList.remove("active-content");
  }
  currentActiveContentId = e;
  var targetElement = document.getElementById(e);
  console.log("targetElement", targetElement);
  targetElement.classList.add("active-content");

  updateBreadcrumb();

  var currentUrl = window.location.href;
  var startIndex = currentUrl.indexOf("?elementId=");
  var newUrl = currentUrl;
  if (startIndex < 0) {
    newUrl += "?elementId=" + e;
  } else {
    var oldElementId = newUrl.substring(
      startIndex + 11,
      currentUrl.length
    );
    newUrl = newUrl.replace(oldElementId, e);
  }
  window.history.pushState("object or string", "Title", newUrl);
}

function setupTree() {
  const urlSearchParams = new URLSearchParams(window.location.search);
  const params = Object.fromEntries(urlSearchParams.entries());
  console.log("params", params);

  filterBox = document.getElementById("filter-box");
  filterBox.addEventListener("input", filterTree);

  TreeConfig.open_icon = '<i class="fas fa-angle-down"></i>';
  TreeConfig.close_icon = '<i class="fas fa-angle-right"></i>';

  TreeConfig.open_icon = '<i class="fas fa-angle-down"></i>';
  TreeConfig.close_icon = '<i class="fas fa-angle-right"></i>';
  TreeConfig.parent_icon = "";
  TreeConfig.showRoot = false;

  treeView.changeOption("show_root", false);
  if (params["elementId"] !== undefined) {
    selectNodeFromParams(root, params["elementId"].toLocaleLowerCase());
  }
  console.log("treeview", treeView);
  treeView.reload();

  updateBreadcrumb();
}

function updateBreadcrumb() {
  var selectedNodes = treeView.getSelectedNodes();
  var breadcrumbElement = document.getElementById("breadcrumb");
  console.log("breadcrumbElement", breadcrumbElement);
  console.log("selectedNodes", selectedNodes);
  console.log("breadcrumbElement.children", breadcrumbElement.children);

  breadcrumbElement.innerHTML = "";
  if (
    selectedNodes == null ||
    selectedNodes == undefined ||
    selectedNodes.length == 0
  ) {
    return;
  }
  var breadcrumbLinks = [];
  var currentNode = selectedNodes[0];
  while (currentNode !== undefined && currentNode !== null) {
    breadcrumbLinks.push(currentNode.getUserObject());
    currentNode = currentNode.parent;
  }
  for (var i = breadcrumbLinks.length - 2; i >= 0; --i) {
    // breadcrumbElement.innerHTML += "<li class=\"breadcrumb-item\"><a href=\"#\" onclick=\"goToElement(" + breadcrumbLinks[i] + ");\">"+breadcrumbLinks[i]+"</a></li>"
    breadcrumbElement.innerHTML +=
      '<li class="breadcrumb-item">' + breadcrumbLinks[i] + "</a></li>";
  }
  console.log("breadcrumblinks", breadcrumbLinks);
}

function goToElement(elementId) {
  selectNodeFromParams(root, elementId);
}

// elementId is a path to the element lik Game.Classes.AnimationHandler
function selectNodeFromParams(node, elementId) {
  var indexOfFirstDot = elementId.indexOf(".");
  var userObject = elementId;
  var isLeaf = true;
  if (indexOfFirstDot >= 0) {
    elementId = elementId.substring(indexOfFirstDot + 1);
    userObject = elementId.substring(0, indexOfFirstDot);
    isLeaf = false;
  }

  if (node.getUserObject().toLowerCase() == userObject) {
    if (isLeaf) {
      node.setSelected(true);
      node.setExpanded(true);
    } else {
      node.getChildren().forEach((n) => {
        if (selectNodeFromParams(n, elementId)) {
          // do nothing?
        }
      });
    }
    return true;
  }
  var isActive = false;
  node.getChildren().forEach((n) => {
    if (selectNodeFromParams(n, elementId)) {
      isActive = true;
    }
  });
  node.setExpanded(isActive);
  return isActive;
}

function filterTree(e) {
  filterText = e.target.value;
  n = treeView.getRoot();
  filterNode(n, filterText.toLowerCase());
  treeView.reload();
}

function filterNode(node, filterText) {
  var isActive = false;

  var nodeText = node.getUserObject().toLowerCase();
  console.log("filtering", nodeText);
  var isActive = isActive || nodeText.includes(filterText) || filterText == "";

  node.setEnabled(true);
  node.getChildren().forEach((n) => {
    var childActive = filterNode(n, filterText);
    if (childActive) {
      isActive = true;
    }
  });

  console.log("enabling", isActive, nodeText);
  node.setEnabled(isActive);
  node.setExpanded(isActive);
  return isActive;
}
