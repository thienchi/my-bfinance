<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="dangdongcmm.cmm.login" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Content management modul (CMM), powered by DANGDONG</title>
    <meta name="CODE_LANGUAGE" content="C#" />
	<meta name="vs_defaultClientScript" content="JavaScript" />
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
	<link type="text/css" rel="stylesheet" href="cssscript/login.css" />
    <script type="text/javascript" src="cssscript/common.js" ></script>
    <script type="text/javascript" src="cssscript/dhtmlwindow.js"></script>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <AJAXTOOLKIT:TOOLKITSCRIPTMANAGER runat="server" ID="ScriptManager1">
        <SERVICES>
            <ASP:SERVICEREFERENCE Path="~/wservice/UtiMailService.asmx" />
        </SERVICES>
    </AJAXTOOLKIT:TOOLKITSCRIPTMANAGER>
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
		    <div id="FLOUT0" align="center">
				<div id="FLOUT0B" align="center">
					<div id="FLOUT1">
						<div id="FLOUT1H">
                            <div class="dgray-no-bottom">
		                        <div style="padding:5px;"><ASP:LABEL ID="lblTitle" runat="server" Text="" CssClass="title1"></ASP:LABEL></div>
                            </div>
                            <div style="padding:5px;"><ASP:HYPERLINK ID="lnkWebsite" runat="server" CssClass="website"></ASP:HYPERLINK></div>
						</div>
				
						<div id="FLOUT1B">
                            <div style="height:250px">
                                <div style="width:110px; position:relative;float:left; padding-top:20px;" align="center"><img src="images/info.png" alt="" /></div>
                                <div style="width:360px; position:relative;float:left">
                                <ASP:PANEL ID="pnlLogin" runat="server" CssClass="tformin">
                                    <div class="FLOUTNOTICE"><ASP:LABEL ID="lblError" runat="server" CssClass="error"></ASP:LABEL></div>
                                    <div class="tformin tformincom">
                                        <div class="rowhor">
                                            <div class="title"><ASP:LABEL ID="lblUsername" runat="server" Text="Tên truy cập"></ASP:LABEL>:</div>
                                            <div class="boxctrll"><ASP:TEXTBOX ID="txtUsername" runat="server"></ASP:TEXTBOX></div>
                                        </div>
                                        <div class="rowhor">
                                            <div class="title"><ASP:LABEL ID="lblPassword" runat="server" Text="Mật khẩu"></ASP:LABEL>:</div>
                                            <div class="boxctrll"><ASP:TEXTBOX ID="txtPassword" runat="server" TextMode="Password"></ASP:TEXTBOX></div>
                                        </div>
                                        <div class="rowhor">
                                            <div class="title"></div>
                                            <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlLang" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLang_SelectedIndexChanged"></ASP:DROPDOWNLIST></div>
                                        </div>
                                    </div>    
        
                                    <div class="tformin tformincom">
                                        <div class="rowhor">
                                            <div class="title"></div>
                                            <div class="boxctrll">
                                                <fieldset>
                                                    <legend>
                                                        <ASP:BUTTON ID="cmdLogin" runat="server" Text="Đăng nhập" CssClass="button" OnClientClick="javascript:return doSubmit();" OnClick="cmdLogin_Click" />
                                                    </legend>
                                                    <div>
                                                        <ASP:CHECKBOX ID="chkRememberlogin" runat="server" Text="Ghi nhớ đăng nhập?" />
                                                        |
                                                        <ASP:LINKBUTTON ID="lnkGetpassword" runat="server" Text="Quên mật khẩu" OnClick="lnkGetpassword_Click"></ASP:LINKBUTTON>
                                                    </div>    
                                                </fieldset>
                                            </div>
                                        </div>
                                    </div>
			                    </ASP:PANEL>
			                    
			                    <ASP:PANEL ID="pnlGetpassword" runat="server" Visible="false" CssClass="tformin">
			                        <div class="FLOUTNOTICE"><ASP:LABEL ID="gp_lblError" runat="server" CssClass="error"></ASP:LABEL></div>
                                    <div class="tformin tformincom">
                                        <div class="rowhor">
                                            <div class="title"><ASP:LABEL ID="gp_lblUsername" runat="server" Text="Tên truy cập"></ASP:LABEL>:</div>
                                            <div class="boxctrll"><ASP:TEXTBOX ID="gp_txtUsername" runat="server"></ASP:TEXTBOX></div>
                                        </div>
                                        <div class="rowhor">
                                            <div class="title"><ASP:LABEL ID="gp_lblEmail" runat="server" Text="E-mail"></ASP:LABEL>:</div>
                                            <div class="boxctrll"><ASP:TEXTBOX ID="gp_txtEmail" runat="server"></ASP:TEXTBOX></div>
                                        </div>
                                    </div>
                                    
                                    <div class="tformin tformincom">
                                        <div class="rowhor">
                                            <div class="title"></div>
                                            <div class="boxctrll">
                                                <fieldset>
                                                    <legend>
                                                        <ASP:BUTTON ID="cmdGetpassword" runat="server" Text="Yêu cầu mật khẩu" CssClass="button" OnClientClick="javascript:return doSubmit();" OnClick="cmdGetpassword_Click" />
                                                    </legend>
                                                    <div>
                                                        <ASP:LINKBUTTON ID="lnkLogin" runat="server" Text="Đăng nhập quản trị" OnClick="lnkLogin_Click"></ASP:LINKBUTTON>
                                                    </div>    
                                                </fieldset>
                                            </div>
                                        </div>
                                    </div>
			                    
			                    </ASP:PANEL>
			                    </div>
				            </div>
						</div>
						<script type="text/javascript">
                        function Createuser(){
                            dangdongcmm.UtiMailService.Createuser(Createuser_Success, onFailed);
                        }
                        function Createuser_Success(e) {  }
                        function onFailed() {  }
						</script>
					
					    <div id="FLOUT1F">
                            <div class="copyright">
                                <ASP:LABEL ID="lblCopyright" runat="server"></ASP:LABEL>
                            </div>
					    </div>
					</div>
					<div style="clear:both"></div>
				</div>
		    </div>
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    </form>
</body>
</html>
