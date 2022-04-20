<%@ Page Title="Parâmetros do Formulário de Viagem" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ParametroViagem.aspx.cs" Inherits="eVidaIntranet.Admin.ParametroViagem" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
            var formatPreco = {
                prefix: '',
                centsSeparator: ',',
                thousandsSeparator: '.',
                centsLimit: 2,
                clearPrefix: true,
                allowNegative: false
            };

            $('#<%= txtDValor.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtKValor.ClientID %>').priceFormat(formatPreco);

            $('.inteiro').each(function () {
                $('#' + this.id).ForceNumericOnly();
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="750px" cellspacing="1px" cellpadding="10px" style="padding-left:75px" >
        <tr>
            <td colspan="5"><h1>VALOR DA DIÁRIA</h1></td>
        </tr>
        <tr>
            <td><b>Seq</b></td>
            <td><b>Início</b></td>
            <td><b>Fim</b></td>
            <td><b>Valor</b></td>
            <td rowspan="2">
                <asp:Button ID="btnDLimpar" runat="server" Text="Limpar" OnClick="btnDLimpar_Click" />         
                <asp:Button ID="btnDSalvar" runat="server" Text="Incluir" OnClick="btnDSalvar_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDSeq" runat="server" Text="-" Width="50px" />
            </td>
            <td>
                <asp:TextBox ID="txtDInicio" runat="server" CssClass="calendario" MaxLength="10" Width="120px" />
            </td>
            <td>
                <asp:TextBox ID="txtDFim" runat="server" CssClass="calendario" MaxLength="10" Width="120px" />
            </td>
            <td>
                <asp:TextBox ID="txtDValor" runat="server" Width="120px" MaxLength="8" />
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <asp:GridView ID="gdvDiaria" runat="server" DataKeyNames="IdLinha" AutoGenerateColumns="false" Width="700px" CssClass="tabela">
                    <Columns>
                        <asp:BoundField HeaderText="Seq" DataField="IdLinha" />
                        <asp:BoundField HeaderText="Início" DataField="Inicio" DataFormatString="{0:dd/MM/yyyy}"/>
                        <asp:BoundField HeaderText="Fim" DataField="Fim" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField HeaderText="Valor" DataField="Value" DataFormatString="{0:C}" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnDEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px"
										OnClick="btnDEditar_Click"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        Não existem parâmetros cadastrados
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <hr />

    <table width="750px" cellspacing="1px" cellpadding="10px" style="padding-left:75px" >
        <tr>
            <td colspan="5"><h1>VALOR KM</h1></td>
        </tr>
        <tr>
            <td><b>Seq</b></td>
            <td><b>Início</b></td>
            <td><b>Fim</b></td>
            <td><b>Valor</b></td>
            <td rowspan="2">
                <asp:Button ID="btnKLimpar" runat="server" Text="Limpar" OnClick="btnKLimpar_Click" />
                <asp:Button ID="btnKSalvar" runat="server" Text="Incluir" OnClick="btnKSalvar_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblKSeq" runat="server" Text="-" Width="50px" />
            </td>
            <td>
                <asp:TextBox ID="txtKInicio" runat="server" CssClass="calendario" MaxLength="10" Width="120px" />
            </td>
            <td>
                <asp:TextBox ID="txtKFim" runat="server" CssClass="calendario" MaxLength="10" Width="120px" />
            </td>
            <td>
                <asp:TextBox ID="txtKValor" runat="server" Width="120px" MaxLength="8" />
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <asp:GridView ID="gdvKm" runat="server" DataKeyNames="IdLinha" AutoGenerateColumns="false" Width="700px" CssClass="tabela">
                    <Columns>
                        <asp:BoundField HeaderText="Seq" DataField="IdLinha" />
                        <asp:BoundField HeaderText="Início" DataField="Inicio" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField HeaderText="Fim" DataField="Fim" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField HeaderText="Valor" DataField="Value" DataFormatString="{0:C}" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnKEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px"
										OnClick="btnKEditar_Click"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        Não existem parâmetros cadastrados
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
