// JScript File

function CC_gotoUrl(url){
    location.href = url;
}
function CC_gotoUrlcountdown(url){
    VALIDATA.Showerroradd('null', VALIDATA.Geterror(VALIDATA.Notice, Definephrase.Notice_gotoUrldelayl));
    var period = 5;
    setTimeout('CC_gotoUrl("'+url+'")', period*1000);
    setTimeout('Countdown('+period+')', 1000);
}
function Countdown(period){
    period--;
    $('#secCountdown').html(period);
    if(period == 0) return;
    setTimeout('Countdown('+period+')', 1000);
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

function showModal(behavior){
    $(document).ready(function(){                
        var modalPopupBehavior = $find(behavior);
        modalPopupBehavior.show();
        return false;
      });
}
function hideModal(behavior){
    var modalPopupBehavior = $find(behavior);
    modalPopupBehavior.hide();
    return false;
}
function hideModaltoProccessing(behavior){
    var modalPopupBehavior = $find(behavior);
    modalPopupBehavior.hide();
    prompt_processing();
    return true;
}


function prompt_processing(){
    dhtmlmodal.openveil()
}
function close_processing(){
    dhtmlmodal.closeveil()
}
function doSubmit(){
    prompt_processing();
    return true;
}
function doChange(){
    prompt_processing();
    return true;
}
function doSavedata() {
    if(!doSave()) return false;
    prompt_processing();
    return true;
}
function doSubmitDel(){
	if(!hasSelected()) return false;
	if(!confirm(Get_Definephrase(Definephrase.Remove_confirm))) return false;
	prompt_processing();
    return true;
}
function doSubmitSel(){
    if(!hasSelected()) return false;
    prompt_processing();
    return true;
}
