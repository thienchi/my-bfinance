// JScript File

function CC_Getform(){
	var theform;
	if(window.navigator.appName.toLowerCase().indexOf("netscape")>-1){
		theform = document.forms["aspnetForm"];
	}else{
		theform = document.aspnetForm;
	}
	return theform;
}
function CC_Getelement(id) {
	var theelem;
	if(window.navigator.appName.toLowerCase().indexOf("netscape")>-1){
		theelem = document.getElementById(id);
	}else{
		var theform = CC_Getform();
		theelem = theform.elements(id);
	}
	return theelem;
}

function trim(str) {
	return str.replace(/^\s+|\s+$/g,"");
}

function getQuerystring(key, default_){
  if (default_==null) default_=""; 
  key = key.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
  var regex = new RegExp("[\\?&]"+key+"=([^&#]*)");
  var qs = regex.exec(window.location.href);
  if(qs == null)
    return default_;
  else
    return qs[1];
} 

function CC_goTop(){
    $("#ctl00_Search_txtKeywords").focus();
}

function switchLang(lang){
    var page = lang + location.href.substring(location.href.lastIndexOf('/'));
    location.href = "../" + page;
}

function prompt_processing(){
    dhtmlmodal.openveil()
}
function close_processing(){
    dhtmlmodal.closeveil()
}

function doSavedata(){
    prompt_processing();
    if(!doSave()){
        close_processing();
        return false;
    }
    return true;
}

var WEBUSERLOGIN = 0;
function MB_WEBUSERLOGIN(logined){
    WEBUSERLOGIN = logined;
}
function MB_DOWNLOADLINK(des){
    return "download-" + des + ".aspx";
}
function MB_Download(des) {
    if(WEBUSERLOGIN==0)
        $("#downloadError").html("Bạn cần <a href='login.aspx'>đăng nhập</a> để tải tập tin này. Nếu chưa có tài khoản, hãy <a href='register.aspx'>đăng ký thành viên</a>");
    else
        window.open(MB_DOWNLOADLINK(des));
}

function restFinput(e, text) {
    $(e).focus(function() {
        if($(e).val()==text) $(e).val('');
    });
    $(e).blur(function() {
        if($(e).val()=='') $(e).val(text);
    });
}

function gotoPage(p) {
    location.href = p;
}