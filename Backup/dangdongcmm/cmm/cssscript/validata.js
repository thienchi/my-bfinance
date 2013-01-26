// JScript File



//check data
var charExp = /./
var letterExp = /[a-z]/i
var phoneExp =  /^\d{10}$/
var memberExp = /^\d{3}$/
var zipExp = /^\d{5}$|^\d{5}[\-\s]?\d{4}$/
var emailExp = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/
var pledgExp = /^\d*$|^\d*\.\d{2}$/
var userExp = /^[a-z][a-z_0-9\.]+[a-z_0-9\.]$/

var VALIDATA = {
    Prefix: '#ctl00_ContentPlaceHolder1_',
    lblError: 'ctl00_lblError',
    //properties
    Notice: 'notice',
    Warning: 'warning',
    Completed: 'completed',
    Error: 'error',
    
    Separator: '#',
    
    //methods
    IsNullOrEmpty:function(param){
        if(param == null || $.trim(param) == '') return true;
	    return false;
    },
    IsNumber:function(param){
        return !isNaN(param);
    },
    IsDate:function(param){
        if(this.IsNullOrEmpty(param)) return false;
        var minYear = 1900;
        var maxYear = 3000;
        re = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/;
        if(regs = param.match(re)) {
            if(regs[1] < 1 || regs[1] > 31) return false;
            else if(regs[2] < 1 || regs[2] > 12) return false;
            else if(regs[3] < minYear || regs[3] > maxYear) return false;
        } 
        else return false;
        return true;
    },
    IsAlpha:function(param){
        return userExp.test(param)
    },
    IsEmail:function(param){
		return emailExp.test(param);
    },

    GetObj:function(e){
	    return $('#' + e);
    },
    GetVal:function(e){
	    return e.attr('value');
    },
    GetIid:function(e){
	    return e.attr('id');
    },
    Gettxt:function(txt, e){
        txt += this.Separator + this.GetIid(e);
	    return txt;
    },
    Getmsg:function(msg, e, errortype, phrase){
        msg += this.Geterror(errortype, phrase);
	    return msg;
    },
    
    Init: function(){

    },
    Geterror: function(errortype, tag){
        var msg = '<div class="' + errortype + '">' + Get_Definephrase(tag) + '</div>';
        return msg;
    },
    Showerror: function(txt, msg){
        this.Clearerrorform();
        if(this.IsNullOrEmpty(txt) || this.IsNullOrEmpty(msg)) return false;
        $('#ctl00_pnlError').show();
        $('#' + this.lblError).html(msg).show();
        $('html, body').animate( { scrollTop: $('#' + this.lblError).offset().top - 100 }, 'slow' );
        
	    var arr = txt.split(this.Separator);
	    if (arr != null && arr.length > 0) 
	        for(var i=1; i < arr.length; i++)
	            $('#' + arr[i]).css('background-color', '#fffbc6');
	    return true;
    },
    Showerroradd: function(txt, msg) {
        if(this.IsNullOrEmpty(txt) || this.IsNullOrEmpty(msg)) return false;
        $('#ctl00_pnlError').show().focus();
        $('#' + this.lblError).html($('#' + this.lblError).html() + msg).show();

        var arr = txt.split(this.Separator);
        if (arr == null || arr.length == 0) return true;
        for (var i = 1; i < arr.length; i++) {
            $("#" + arr[i]).css("background-color", "#fffbc6");
        }
        return true;
    },
    Clearerror:function(){
        theForm.reset();
        $('#ctl00_pnlError').hide();
        $('#' + this.lblError).html('');
        $('.tformin').find('input[type="text"]').css('background-color', '#ffffff');
        $('.tformin').find('textarea').css('background-color', '#ffffff');
        $('.tformin').find('select').css('background-color', '#ffffff');
	    return false;
    },
    Clearerrorform:function(){
        $('#' + this.lblError).html('');
        $('.tformin').find('input[type="text"]').css('background-color', '#ffffff');
        $('.tformin').find('textarea').css('background-color', '#ffffff');
        $('.tformin').find('select').css('background-color', '#ffffff');
	    return false;
    }
    
}




var Definephrase = {
    Require: '_Require',
    Require_code : '_Require_code',
    Require_name: '_Require_name',
    Require_nick: '_Require_nick',
    Require_namefl : '_Require_namefl',
    Require_username : '_Require_username',
    Require_password : '_Require_password',
    Require_email : '_Require_email',
    Require_title : '_Require_title',
    Require_content : '_Require_content',
    Require_catalogue: '_Require_catalogue',
    Require_catalogueinfo: '_Require_catalogueinfo',
    Require_keyword: '_Require_keyword',
    Require_menu : '_Require_menu',

    Invalid: '_Invalid',
    Invalid_right: '_Invalid_right',
    Invalid_data: '_Invalid_data',
    Invalid_int: '_Invalid_int',
    Invalid_datetime: '_Invalid_datetime',
    Invalid_filesize: '_Invalid_filesize',
    Invalid_filepreviewsize: '_Invalid_filepreviewsize',
    Invalid_fileurl: '_Invalid_fileurl',
    Invalid_file: '_Invalid_file',
    Invalid_username: '_Invalid_username',
    Invalid_username_notexist: '_Invalid_username_notexist',
    Invalid_username_email: '_Invalid_username_email',
    Invalid_password: '_Invalid_password',
    Invalid_email: '_Invalid_email',

    Remove_confirm: '_Remove_confirm',
    Remove_denied: '_Remove_denied',
    Remove_completed: '_Remove_completed',
    Remove_error: '_Remove_error',
    

    Save_notice: '_Save_notice',
    Save_completed: '_Save_completed',
    Save_error: '_Save_error',
    Copy_completed: '_Copy_completed',
    Copy_error: '_Copy_error',
    Move_completed: '_Move_completed',
    Move_error: '_Move_error',
    Saveorderd_completed: '_Saveorderd_completed',
    Saveorderd_error: '_Saveorderd_error',
    Removefilepreview_confirm: '_Removefilepreview_confirm',
    Removefilepreview_completed: '_Removefilepreview_completed',
    Removefilepreview_error: '_Removefilepreview_error',

    Firstitem_ddl: '_Firstitem_ddl',
    Task_Require_Assigned: '_Task_Require_Assigned',
    
    Notice_noselecteditem: '_Notice_noselecteditem',
    Notice_gotoUrldelay: '_Notice_gotoUrldelay',
    Notice_gotoUrldelayl: '_Notice_gotoUrldelayl',
    Display_pis: '_Display_pis',
    Display_havesub: '_Display_havesub'
}

var Ffile = "../xhtml/Definephrase.xml";
function Get_Definephrase(tag){
	var xmlDoc=null;
	if(window.ActiveXObject){
		xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
	}else if(document.implementation.createDocument){
		xmlDoc=document.implementation.createDocument("","",null);
	}else return;
	if(xmlDoc!=null){
		xmlDoc.async=false;
		xmlDoc.load(Ffile);
		var x=xmlDoc.getElementsByTagName("phrase");
		for(i=0;i<x.length;i++){
			return x[i].getElementsByTagName(LANG+tag)[0].childNodes[0].nodeValue;
		}
	}
}

