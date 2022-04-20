<%@ Page Title="Intranet E-VIDA" Language="C#" MasterPageFile="~/Internal/Internal.Master"  AutoEventWireup="true" CodeBehind="Inicial.aspx.cs" Inherits="eVidaIntranet.Inicial" %>
<%@ Register TagPrefix="evida" TagName="Menu" Src="~/controls/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
	    $(document).ready(function () {

	    });

	</script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div style="height:500px" >
	<h2>Seja bem vindo!</h2>
        <img src="../img/logo.png" alt="E-Vida Intranet" />
    </div>
</asp:Content>