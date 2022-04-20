<%@ Page Title="Relatório de Despesas CEA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioDespesasCEA.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioDespesasCEA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <iframe width="975" height="732" src="https://app.powerbi.com/view?r=eyJrIjoiMTQyZjJmZjItODA4Yy00MTZmLWE4YzgtODQ1MzEwMzRlZWVjIiwidCI6Ijk0Nzk1ODI4LTBjNGQtNDA5Ny04MWNlLWNiZjA1ZGJhMGU1NCJ9" frameborder="0" allowFullScreen="true"></iframe>
</asp:Content>
