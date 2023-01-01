function ToogleList() {
    document.getElementById("OpenList").classList.toggle("Hide");
    document.getElementById("CloseList").classList.toggle("Hide");
    document.getElementById("SideBar").classList.toggle("SideBarShow");
    document.getElementById("ToogleList").classList.toggle("HeaderListBarShow");
    var Status = document.getElementById("OpenList").classList.value;
}