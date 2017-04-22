var selectedProject = null;

function main() {
    // handle window drag
    window.onmousemove = function () {
        window.windowEvents.mouseMove();
    };

    Element.prototype.remove = function () {
        this.parentElement.removeChild(this);
    }
    NodeList.prototype.remove = HTMLCollection.prototype.remove = function () {
        for (var i = this.length - 1; i >= 0; i--) {
            if (this[i] && this[i].parentElement) {
                this[i].parentElement.removeChild(this[i]);
            }
        }
    }

    // load projects
    window.projectEvents.loadProjects();
}

function tests() {
    addProject("MURDERSHADOW");
    addProject("Test");
    addProject("SampleProject");

    selectProject("MURDERSHADOW");

    addFileChange("README.md", 0);
    addFileChange("readme.md", 2);
    addFileChange("assets/models/characters/char_default.fbx", 1);
    addFileChange("assets/materials/characters/char_default.mat", 1);
    addFileChange("assets/materials/characters/char_default_body.mat", 1);
    addFileChange("assets/materials/characters/char_default_eyes.mat", 1);
    addFileChange("assets/materials/characters/char_default_clothes.mat", 1);
    addFileChange("assets/materials/characters/char_default_clothes1.mat", 2);
    addFileChange("assets/textures/characters/char_default_body.tga", 1);
    addFileChange("assets/textures/characters/char_default_eyes.tga", 1);
    addFileChange("assets/textures/characters/char_default_clothes.tga", 1);
    addFileChange("assets/resources/weapons.asset", 1);
    addFileChange("assets/scenes/loading.unity", 0);
    addFileChange("assets/scenes/main.unity", 2);

    setChangeCount("MURDERSHADOW", 14);

    toggleFileStage("README.md");
    toggleFileStage("readme.md");
    toggleFileStage("assets/scenes/loading.unity");
    toggleFileStage("assets/scenes/main.unity");
}


// project management methods

function addProject(projectname) {
    var view = document.getElementById("projects_list_view");

    var html =
        "<div id='proj_"+projectname+"' onclick=\"selectProject('"+projectname+"', true);\" class='projects_list_item projects_list_item_notactive'>" +
        "<div class='projects_list_item_title'>"+projectname+"</div>" +
        "<div id='proj_" + projectname + "_chcount' class='projects_list_item_chcount'>0 change(s)</div>" +
        "<div class='projects_list_item_delete'></div>" +
        "</div>";

    view.innerHTML += html;
}

function removeProject(projectname) {
    var elem = document.getElementById(`proj_${projectname}`);
    elem.remove();
}

function setChangeCount(projectname, changecount) {
    var elem = document.getElementById(`proj_${projectname}_chcount`);
    elem.innerHTML = changecount + " change(s)";
}

function selectProject(projectname, local) {
    if (selectedProject != null) {
        selectedProject.className = "projects_list_item projects_list_item_notactive";
    }

    selectedProject = document.getElementById(`proj_${projectname}`);
    selectedProject.className = "projects_list_item projects_list_item_active";

    // clear file changes
    clearFileChanges();

    // send c# callback
    if (local)
        window.projectEvents.selectProject(projectname);
}


function clearFileChanges() {
    var elem = document.getElementById("files_list_view");
    elem.innerHTML = "";
}

function addFileChange(name, state) {
    var elem = document.getElementById("files_list_view");
    
    if (state === 0) {
        elem.innerHTML += `<div id='c_${name}' class='files_list_item files_list_item_new' onclick='toggleFileStage(\"${name}\");'>${name}</div>`;
    } else if (state === 1) {
        elem.innerHTML += `<div id='c_${name}' class='files_list_item files_list_item_changed' onclick='toggleFileStage(\"${name}\");'>${name}</div>`;
    } else {
        elem.innerHTML += `<div id='c_${name}' class='files_list_item files_list_item_deleted' onclick='toggleFileStage(\"${name}\");'>${name}</div>`;
    }
}

function toggleFileStage(name) {
    var elem = document.getElementById(`c_${name}`);
    
    if (elem.className.endsWith("files_list_item_selected")) {
        elem.className = elem.className.replace(" files_list_item_selected", "");
    } else {
        elem.className += " files_list_item_selected";
    }
}

function getSelectedFiles() {
    var elem = document.getElementById("files_list_view");
    var nodes = elem.childNodes;

    var selected = [];

    nodes.forEach(function (item, index) {
        if (item != null && item.className != undefined) {
            if (item.className.endsWith("files_list_item_selected")) {
                // this is selected
                selected.push(item.innerHTML);
            }
        }
    });

    return selected;
}

function selectAll() {
    var elem = document.getElementById("files_list_view");
    var nodes = elem.childNodes;

    nodes.forEach(function (item) {
        if (item != null && item.className != undefined) {
            if (!item.className.endsWith("files_list_item_selected")) {
                // this is selected
                item.className += " files_list_item_selected";
            }
        }
    });
}

function unselectAll() {
    var elem = document.getElementById("files_list_view");
    var nodes = elem.childNodes;

    nodes.forEach(function (item) {
        if (item != null && item.className != undefined) {
            if (item.className.endsWith("files_list_item_selected")) {
                // this is selected
                item.className = item.className.replace(" files_list_item_selected", "");
            }
        }
    });
}

// modal windows management methods

function showMessage(message, isError) {
    if (isError) {
        document.getElementById("message_overlay_window").className = "message_overlay_window message_overlay_window_error";
    } else {
        document.getElementById("message_overlay_window").className = "message_overlay_window";
    }

    document.getElementById("message_overlay").style = "display: block;";
    document.getElementById("message_overlay_message").innerHTML = message;
}

function hideMessage() {
    document.getElementById("message_overlay").style = "display: none;";
}


// modal windows management methods

function showProgress() {
    document.getElementById("progress_overlay").style = "display: block;";
}

function setProgressMesssage(message) {
    document.getElementById("progress_overlay_window").innerHTML = message;
}

function hideProgress() {
    document.getElementById("progress_overlay").style = "display: none;";
}

// run main
main();

//tests();