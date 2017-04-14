﻿var selectedProject = null;

// handle window drag
window.onmousemove = function() {
    window.jswindow.mouseMove();
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


// project management methods

// load projects
window.projects.loadProjects();


function addProject(projectname) {
    var view = document.getElementById("projects_list_view");

    var html =
        "<div id='proj_"+projectname+"' onclick=\"selectProject('"+projectname+"');\" class='projects_list_item projects_list_item_notactive'>" +
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

function selectProject(projectname) {
    if (selectedProject != null) {
        selectedProject.className = "projects_list_item projects_list_item_notactive";
    }

    selectedProject = document.getElementById(`proj_${projectname}`);
    selectedProject.className = "projects_list_item projects_list_item_active";

    // send c# callback
    window.projects.selectProject(projectname);
}


function clearFileChanges() {
    var elem = document.getElementById("files_list_view");
    elem.innerHTML = "";
}

function addFileChange(name, state) {
    var elem = document.getElementById("files_list_view");
    
    if (state === 0) {
        elem.innerHTML += `<div class='files_list_item files_list_item_new' onclick='toggleFileStage(\" ${name} \");'> ${name} </div>`;
    }
    else if (state === 1) {
        elem.innerHTML += `<div class='files_list_item files_list_item_changed' onclick='toggleFileStage(\" ${name} \");'> ${name} </div>`;
    } else {
        elem.innerHTML += `<div class='files_list_item files_list_item_deleted' onclick='toggleFileStage(\" ${name} \");'> ${name} </div>`;
    }
}

function toggleFileStage(filename) {
    
}

function getSelectedFiles() {
    
}