<%@ Page Language="C#" MasterPageFile="MasterHome.Master" AutoEventWireup="true" CodeBehind="confirmation.aspx.cs" Inherits="dangdongcmm.confirmation" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="COMMONNOTICE erroricon">
        <ASP:LABEL ID="lblError" runat="server"></ASP:LABEL>
    </div>

    <script type="text/javascript">
    function Resetpassword(){
        dangdongcmm.UtiMailService.Resetpassword(Resetpassword_Success, onFailed);
    }
    function Resetpassword_Success(e) {  }
    function onFailed() {  }
    function Regconfirm(){
        dangdongcmm.UtiMailService.Regconfirm(Regconfirm_Success, onFailed);
    }
    function Regconfirm_Success(e) {  }

    </script>
    
</ASP:CONTENT>