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
    lblError: '',
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
	    return e.val();
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
    
    Init:function(){

    },
    Geterror:function(errortype, tag){
        var msg = '<div class="' + errortype + '">' + Get_Definephrase(tag) + '</div>';
        return msg;
    },
    Showerror:function(txt, msg){
        this.Clearerrorform();
        if(this.IsNullOrEmpty(txt) || this.IsNullOrEmpty(msg)) return false;
        $('#' + this.lblError).html(msg).show();
        $('html, body').animate( { scrollTop: $('#' + this.lblError).offset().top - 100 }, 'slow' );
        
	    var arr = txt.split(this.Separator);
	    if(arr==null || arr.length==0) return true;
	    for(var i=1; i < arr.length; i++){
	        var e = $("#" + arr[i]);
	        e.css("background-color","#fffbc6");
	    }
	    return true;
    },
    Clearerror:function(){
        theForm.reset();
        $('#' + this.lblError).html('');
        $('.tformin').find('input[type="text"]').css("background-color","#ffffff");
        $('.tformin').find('textarea').css("background-color","#ffffff");
	    return false;
    },
    Clearerrorform:function(){
        $('#' + this.lblError).html('');
        $('.tformin').find('input[type="text"]').css("background-color","#ffffff");
        $('.tformin').find('textarea').css("background-color","#ffffff");
	    return false;
    }
    
}




var Definephrase = {
    Require: '_Require',
    Require_namefl: '_Require_namefl',
    Require_title: '_Require_title',
    Require_email: '_Require_email',
    Require_password: '_Require_password',
    Require_content: '_Require_content',
    Require_bank: '_Require_bank',
    Require_accountname: '_Require_accountname',
    Require_accountnumber: '_Require_accountnumber',
    Invalid: '_Invalid',
    Invalid_email: '_Invalid_email',
    Invalid_username: '_Invalid_username',
    Invalid_password: '_Invalid_password',
    Invalid_passwordretype: '_Invalid_passwordretype'
}

function get_lang(){
	var path = window.location.href;
	var i = path.lastIndexOf('/');
	var flang = path.substring(i-2, i);
	return flang;
}
//var LANG = get_lang();

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

