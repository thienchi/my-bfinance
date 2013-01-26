<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="memberu.aspx.cs" Inherits="dangdongcmm.cmm.memberu" %>
<%@ MasterType virtualpath="MasterDefault.master" %>
<%@ REGISTER Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>
<%@ REGISTER TagPrefix="UC" TagName="Filepreview" Src="usercontrol/ucfilepreview.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Displaysetting" Src="usercontrol/ucdisplaysetting.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ul class="tabs">
	        <li class="first"><a href="#">Thông Tin Tài Khoản</a></li>
	        <li><a href="#">Hồ Sơ</a></li>
	        <li><a href="#">Thiết Lập Hiển Thị</a></li>
        </ul>
        <div class="panes">
            <div class="tformin tformincom">      
                <div class="rowhor">
                    <div class="title">Họ:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtFirstname" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Tên:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtLastname" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">E-mail:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtEmail" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Điện thoại:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtPhone" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Loại tài khoản:</div>
                    <div class="boxctrls" style="width:134px;"><ASP:DROPDOWNLIST ID="ddlGrouptype" runat="server">
                        <ASP:LISTITEM>Thành viên</ASP:LISTITEM>
                        <ASP:LISTITEM>Tổng</ASP:LISTITEM>
                    </ASP:DROPDOWNLIST></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Tên tài khoản:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtUsername" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title"><span class="require">*</span> Mật khẩu:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtPassword" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrls" style="margin-left:5px"><ASP:BUTTON ID="cmdCreatepassword" runat="server" Text="Tạo mật khẩu" CssClass="buttoncmd" OnClick="cmdCreatepassword_Click" /></div>
                </div>
            </div>
        
            <div class="tformin tformincom">
                <!--
                <div class="rowhor">
                    <div class="title">District:</div>
                    <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlDistrict" runat="server"></ASP:DROPDOWNLIST></div>
                </div>
                
                <div class="rowhor">
                    <div class="title">Birthdate</div>
                    <div class="boxctrls" style="width:80px;"><ASP:DROPDOWNLIST ID="ddlYear" runat="server"></ASP:DROPDOWNLIST></div>
                    <div class="boxctrls" style="margin-left:10px;">          
                        <ASP:DROPDOWNLIST ID="ddlMonth" runat="server">
                            <ASP:LISTITEM Value="1">January</ASP:LISTITEM>
                            <ASP:LISTITEM Value="2">February</ASP:LISTITEM>
                            <ASP:LISTITEM Value="3">March</ASP:LISTITEM>
                            <ASP:LISTITEM Value="4">April</ASP:LISTITEM>
                            <ASP:LISTITEM Value="5">May</ASP:LISTITEM>
                            <ASP:LISTITEM Value="6">June</ASP:LISTITEM>
                            <ASP:LISTITEM Value="7">July</ASP:LISTITEM>
                            <ASP:LISTITEM Value="8">August</ASP:LISTITEM>
                            <ASP:LISTITEM Value="9">September</ASP:LISTITEM>   
                            <ASP:LISTITEM Value="10">October</ASP:LISTITEM>
                            <ASP:LISTITEM Value="11">November</ASP:LISTITEM>
                            <ASP:LISTITEM Value="12">December</ASP:LISTITEM>
                        </ASP:DROPDOWNLIST>
                     </div>
                     <div class="boxctrls" style="width:62px; margin-left:10px;">  
                        <ASP:DROPDOWNLIST ID="ddlDay" runat="server"></ASP:DROPDOWNLIST>
                     </div>
                </div>
                <div class="rowhor">
                    <div class="title">Address:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtAddress" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">City:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtCity" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">District:</div>
                    <div class="boxctrlc"><ASP:TEXTBOX ID="txtDistrict" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">Mã vùng:</div>
                    <div class="boxctrls"><ASP:TEXTBOX ID="txtZipcode" runat="server"></ASP:TEXTBOX></div>
                </div>
                <div class="rowhor">
                    <div class="title">National:</div>
                    <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlNational" runat="server"></ASP:DROPDOWNLIST></div>
                </div>
                <div class="rowhor">
                    <div class="title">City:</div>
                    <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlCity" runat="server">
                            <ASP:LISTITEM Value="0" class="textdefndis">Select...</ASP:LISTITEM>
                        </ASP:DROPDOWNLIST></div>
                </div>
                -->
                <div class="rowhor">
                    <div class="title">Giới thiệu:</div>
                    <div class="boxctrlm"><ASP:TEXTBOX ID="txtAbout" runat="server" TextMode="MultiLine" Rows="10"></ASP:TEXTBOX></div>
                </div>
                
                <!--
                <div class="dbreakh"></div>
                <div class="rowhor">
                    <div class="title">Blog <img src="../images/blog-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtBlog" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radBlogsh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
                <div class="rowhor">
                    <div class="title">Home page <img src="../images/home-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtHomepage" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radHomepagesh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
                <div class="rowhor">
                    <div class="title">Facebook <img src="../images/facebook-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtFacebook" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radFacebooksh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
                <div class="rowhor">
                    <div class="title">Twitter <img src="../images/twitter-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtTwitter" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radTwittersh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
                <div class="rowhor">
                    <div class="title">Youtube <img src="../images/youtube-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtYoutube" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radYoutubesh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
                <div class="rowhor">
                    <div class="title">Flickr <img src="../images/flickr-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtFlickr" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radFlickrsh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div
                -->
                <div class="dbreakh"></div>
                <div class="rowhor">
                    <div class="title">Skype <img src="../images/skype-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtSkype" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radSkypesh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>
                <div class="rowhor">
                    <div class="title">Yahoo <img src="../images/yahoo-16x16.png" alt="" align="middle" /></div>
                    <div class="boxctrlms"><ASP:TEXTBOX ID="txtYahoo" runat="server"></ASP:TEXTBOX></div>
                    <div class="boxctrlc" style="width:150px; margin-left:30px;">
                        <ASP:RADIOBUTTONLIST ID="radYahoosh" runat="server" CssClass="ctrlrad" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1" Selected="True">Show</ASP:LISTITEM>
                            <ASP:LISTITEM Value="0">Hide</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST>
                    </div>
                </div>

            </div>
            
            <div class="tformin tformincom">
                <UC:DISPLAYSETTING ID="Displaysetting" runat="server" Showdisplayorder="false" Showicon="false" />
            </div>
        </div>
        
        <div class="tformin tformincom">
            <br />&nbsp;
            <div class="rowhor">
                <div class="title"></div>
                <div class="boxctrll">
                    <ASP:HIDDENFIELD ID="txtId" runat="server" Value="0" />
                    <fieldset>
                        <legend>
                            <ASP:BUTTON ID="cmdSave" runat="server" CssClass="button" Text=" Lưu " OnClientClick="javascript:return doSavedata();" OnClick="cmdSave_Click" />
                            <ASP:BUTTON ID="cmdClear" runat="server" CssClass="button" Text="Nhập lại" OnClick="cmdClear_Click" />
                        </legend>
                        <div>
                            <ASP:CHECKBOX ID="chkSaveoption_golist" runat="server" Text="Lưu thông tin & tiếp tục chỉnh sửa?" />
                        </div>    
                    </fieldset>
                </div>
            </div>
        </div>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>

    <script type="text/javascript">
    $(function() { 
        initabs(); 
        var tab = getQuerystring('tab', '');
        //if(tab=='application') gottabs(2);
    });

    function doSave() {
	    var the, txt = '', msg = '';
	    the = VALIDATA.GetObj('<%=txtUsername.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsAlpha(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_username);
        }
	    the = VALIDATA.GetObj('<%=txtPassword.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_password);
        }
	    the = VALIDATA.GetObj('<%=txtEmail.ClientID %>');
        if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) || !VALIDATA.IsEmail(VALIDATA.GetVal(the))){
            txt = VALIDATA.Gettxt(txt, the);
            msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_email);
        }
	    
	    if(VALIDATA.Showerror(txt, msg)) return false;
	    return true;
    }

    function Register(){
        dangdongcmm.UtiMailService.Register(Register_Success, onFailed);
    }
    function Register_Success(e) {  }
    function onFailed() {  }

    </script>
    
</ASP:CONTENT>
