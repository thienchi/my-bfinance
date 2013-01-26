// JScript File


function tglSel(icheckall, icheck){
    var isChecked = $('#' + icheckall).is(':checked');
    $('#' + icheckall).parents('tbody').find('input[type="checkbox"][name="' + icheck + '"]:enabled')
        .each(function() {
            $(this).attr('checked', isChecked);
            var row = $(this).parents('tr');
            if( $(this).is(':checked') )
                $(row).removeClass().addClass( $(row).attr('class').indexOf('rowormal') !=- 1 ? 'rowormalsel' : 'rowaltersel' );
            else
                $(row).removeClass().addClass( $(row).attr('class').indexOf('rowormal') !=- 1 ? 'rowormal' : 'rowalter' );    
        }
    );
}
function selAll(icheck){
	for(var i=0; i<theForm.elements.length; i++){
		e = theForm.elements[i];
		if(e.type=='checkbox' && e.name==icheck && !e.disabled) e.checked = true;
	}
	return;
}
function cleAll(icheck){
	for(var i=0; i<theForm.elements.length; i++){
		e = theForm.elements[i];
		if(e.type=='checkbox' && e.name==icheck && !e.disabled) e.checked = false;
	}
	return;
}
function chkAll(icheckall, icheck){
    if(!$(icheck).is(':checked'))
        $('#' + icheckall).attr('checked', false);
    else {
        var n = $('#' + icheckall).parents('tbody').find('input[type="checkbox"][name="' + $(icheck).attr('name') + '"]:enabled:not(:checked)').length;
        $('#' + icheckall).attr('checked', n > 0 ? false : true);
    }    
    return;
}
function selCss(){return;
	var e = event.srcElement;
	var row = e.parentNode;
	while(row.tagName.toLowerCase()!="tr"){
	    row = row.parentNode;
	}
	if(e.checked)
	    row.className = row.className.indexOf('rowormal')!=-1? 'rowormalsel':'rowaltersel';
	else
	    row.className = row.className.indexOf('rowormal')!=-1? 'rowormal':'rowalter';
}

function getChecked(icheck){
	var arrid = '';
	for(var i=0; i<theForm.elements.length; i++){
	    e = theForm.elements[i];
	    if(e.type=='checkbox' && e.name==icheck && e.checked) arrid += e.value + ",";
	}
	return arrid;
}

function doDelo(iid){
	return confirm(Get_Definephrase(Definephrase.Remove_confirm));
}

function hasSelected() {
    var arrid = '';
    $('input[type="checkbox"][name="chkcheck"]:checked').each(function() {
        arrid += $(this).val() + ',';
    });
    var msg = '';
	if(VALIDATA.IsNullOrEmpty(arrid)) {
	    msg = VALIDATA.Getmsg('', '', VALIDATA.Warning, Definephrase.Notice_noselecteditem);
	}
	if(VALIDATA.Showerror('null', msg)) return false;
	
	return true;
}

function doRemovefilepreview(){
    return confirm(Get_Definephrase(Definephrase.Removefilepreview_confirm));
}
function Removefilepreview_CallBack(response){
	if(response.error!=null){
		alert(response.error);
		return false;
	}
	var vlreturn = response.value;
	if(vlreturn==null || vlreturn==Definephrase.Invalid_right){
	    msg = Get_Error('', Errortype.Notice, Definephrase.Invalid_right);
	    Show_Error('null', msg);
		return false;
	}

    msg = Get_Error('', vlreturn==Definephrase.Removefilepreview_completed? Errortype.Completed:Errortype.Error, vlreturn);
    Show_Error('null', msg);
    return vlreturn==Definephrase.Removefilepreview_completed;
}


function ddlGotopage(e, page){
    location.href = page + '?cid=' + $(e).val();
}
function ddlGotopageBlto(e, page){
    var ddl = CC_Getelement(e.id);
    location.href = page + '?blto=' + ddl.value;
}
function ddlGotopageExti(e, page){
    var ddl = CC_Getelement(e.id);
    location.href = page + "?blto=" + getQuerystring('blto', 0) + "&aid=" + ddl.value;
}
function ddlGotopageSpec(e, page){
    var ddl = CC_Getelement(e.id);
    location.href = page + ddl.value;
}



function lEffect_Over(e){
    while (!$(e).is('tr')) {
        e = e.parent();
    }
    var newclass = $(e).attr('class').indexOf('rowormal') !=- 1 ? 'rowormalsel' : 'rowaltersel';
    $(e).removeClass().addClass( newclass );
} 
function lEffect_Out(e){
    while (!$(e).is('tr')) {
        e = e.parent();
    }
    if( $('#chkcheck' + $(e).attr('id')).is(':checked') ) {
        var newclass = $(e).attr('class').indexOf('rowormal') !=- 1 ? 'rowormalsel' : 'rowaltersel';
        $(e).removeClass().addClass( newclass );
    }    
    else {
        var newclass = $(e).attr('class').indexOf('rowormal') !=- 1 ? 'rowormal' : 'rowalter';
        $(e).removeClass().addClass( newclass );
    }    
}

function lEffect_Outuc(e){
    var ec = CC_Getelement("chkcheckuc" + e.id);
    row = ec.parentNode;
    while (row.tagName.toLowerCase()!="tr"){
        row = row.parentNode;
    }
    if(ec.checked)
        row.className = row.className.indexOf('rowormal')!=-1? 'rowormalsel':'rowaltersel';
    else
        row.className = row.className.indexOf('rowormal')!=-1? 'rowormal':'rowalter';
}

function actionMulti(e) {
    if($(e).val()=='') {return false;}
    var b;
    switch($(e).val()) {
        case 'Delete':
            b = doSubmitDel();
            break;
        case 'Copy':
        case 'Move':
        case 'Setupattribute':
            b = doSubmitSel();
            break;
        case 'Updatedisplayorder':
        case 'Exportexcel':
            b = doSubmit();
            break;
        default:
            b = false;
            break;
    }
    if(b) {__doPostBack('ctl00$ContentPlaceHolder1$ddlAction', '');}
    else {$(e).find('option:first').attr('selected', true);}
}


function UICurrency_In(e){
    var value = UICurrency2Num($(e).val());
    $(e).val( value == 0 ? '' : value.toString() );
}
function UICurrency_Out(e){
    var value = UICurrency2Num($(e).val());
    $(e).val( value != 0 ? ('$ ' + Num2UICurrency(value.toString())) : '$ ' );
}

function UICurrency2Num(param){
    param = param.replace(/^\s+\$|\$|\s+_|_|\s+,|,/g,"");
    if(VALIDATA.IsNullOrEmpty(param)) return 0;
    if(param.indexOf('.') == 0) param = '0' + param;
    if(param.indexOf('.') == param.length-1) param += '00';
    return Math.abs(param);
}
function Num2UICurrency(param){
    if (VALIDATA.IsNullOrEmpty(param)) return '';
    var arrchar = param.toString().split('.');
    var n_head = arrchar[0];
    var n_foot = arrchar.length > 1 ? arrchar[1] : '';
    
    var n_head_result = '';
    if (n_head.length > 3){
        var n_head_mod = n_head.length % 3;
        var n_head_div = parseInt(n_head.length/3);

        for (i = 0; i <= n_head_div; i++){
            if(i==n_head_div && n_head_mod==0) break;
            if(n_head_mod!=0)
                if(i==0)
                    n_head_result += n_head.substring(0, n_head_mod) + ',';
                else
                    n_head_result += n_head.substring((i-1)*3+n_head_mod, (i-1)*3+n_head_mod+3) + ',';
            else
                n_head_result += n_head.substring(i*3, i*3+3) + ',';
        }
        n_head_result = n_head_result.substring(0, n_head_result.length-1);
    }
    return (n_head_result==''? n_head : n_head_result) + (n_foot != "" ? ("." + (n_foot.length==1? n_foot + '0' : n_foot.substring(0,2))) : ".00");
}

function UINumber_In(e){
    var keycode;
	if(window.event) keycode=window.event.keyCode;
	else if(e) keycode=e.which;
	else return true;
	if(moveKey(keycode) || (keycode>47 && keycode<58)){return true;}
	else return false;
}
function UINumber_Out(e){
    var value = $(e).val();
    $(e).val( !VALIDATA.IsNullOrEmpty(value) && VALIDATA.IsNumber(value) ? value : '0' );
}

function moveKey(key){
    return (key==0 || key==8);
}