
currentActiveContentId = "";
filterBox = null;
treeView = null;

function onNodeClick(e) {
  var targetElementOld = document.getElementById(currentActiveContentId);
  if (targetElementOld !== null && targetElementOld !== undefined) {
    targetElementOld.classList.remove('active-content');
  }
  currentActiveContentId = e;
  var targetElement = document.getElementById(e);
  console.log("targetElement", targetElement);
  targetElement.classList.add('active-content');
}

function setupTree() {

  const urlSearchParams = new URLSearchParams(window.location.search);
  const params = Object.fromEntries(urlSearchParams.entries());
  console.log("params", params);

  filterBox = document.getElementById("filter-box");
  filterBox.addEventListener('input', filterTree);

  var count = 1;

  var root = new TreeNode("Zusagenframework");

  var a;

  var n1 = new TreeNode("Bausteinabhängigkeit");
  var n11 = new TreeNode("Api");
  var n111 = new TreeNode("Bausteinberechner");
  n111.on('select', function (e, node) {
    onNodeClick("Bausteinberechner")
  });
  var n12 = new TreeNode("Customizing");
  var n121 = new TreeNode("IBausteinberechnungsCustomizingprovider");
  n121.on('select', function (e, node) {
    onNodeClick("IBausteinberechnungsCustomizingprovider")
  });
  var n13 = new TreeNode("Beschreibung");
  var n131 = new TreeNode("Bausteinberechnungskern");
  var n132 = new TreeNode("Bausteindefinition");


  root.addChild(n1);

  n1.addChild(n11);
  n1.addChild(n12);
  n1.addChild(n13);

  n11.addChild(n111);
  n12.addChild(n121);
  n13.addChild(n131);
  n13.addChild(n132);

  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));
  root.addChild(new TreeNode("Test"));

  treeView = new TreeView(root, "#container");

  TreeConfig.open_icon = '<i class="fas fa-angle-down"></i>';
  TreeConfig.close_icon = '<i class="fas fa-angle-right"></i>';

  TreeConfig.open_icon = '<i class="fas fa-angle-down"></i>';
  TreeConfig.close_icon = '<i class="fas fa-angle-right"></i>';
  TreeConfig.parent_icon = '';

  if (params['elementId'] !== undefined) {
    selectNodeFromParams(root, params['elementId'].toLocaleLowerCase());
  }

  treeView.reload();
}

function selectNodeFromParams(node, elementId) {

  if (node.getUserObject().toLowerCase() == elementId) {
    node.setSelected(true);
    node.setExpanded(true);
    return true;
  }
  var isActive = false;
  node.getChildren().forEach(n => {
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
  var isActive = isActive || (nodeText.includes(filterText) || filterText == "")

  node.setEnabled(true);
  node.getChildren().forEach(n => {
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

setupTree();
