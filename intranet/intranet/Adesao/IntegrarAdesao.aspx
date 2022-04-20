<%@ Page Title="Realizar Integração no Protheus" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="IntegrarAdesao.aspx.cs" Inherits="eVidaIntranet.Adesao.IntegrarAdesao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function goBusca() {
            window.location = 'BuscaAdesoes.aspx';
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    <table width="100%" style="padding:10px">
	    <tr>
		    <td>
			    <asp:HiddenField ID="hidProtocolo" runat="server" />

                <h2>Cabeçalho</h2>
			    <table id="tbDados" class="tbBorda" border="1">
                    <tr>
                        <td><b>Protocolo<br /></b><asp:Literal ID="litProtocolo" runat="server" /></td>
                        <td><b>Empresa<br /></b><asp:Literal ID="litEmpresa" runat="server" /></td>
                        <td><b>Produto</b><br /><asp:Literal ID="litProduto" runat="server" /></td>
                    </tr>
                </table>

                <h2>Titular</h2>
                <table class="tbBorda" border="1">
                    <tr>
                        <td><b>Cartão</b><br /><asp:Literal ID="litIdBeneficiario" runat="server" /></td>
                        <td><b>Matrícula<br /></b><asp:Literal ID="litMatricula" runat="server" /></td>
                        <td><b>Nome<br /></b><asp:Literal ID="litNomeTitular" runat="server" /></td>
                        <td><b>CPF<br /></b><asp:Literal ID="litCpfTitular" runat="server" /></td>
                    </tr>
                
                </table>

                <h2>Dependentes</h2>
                <table class="tbBorda" border="1">
                    <tr>
                        <td>
                            <b>Dependentes são validados no Protheus pelo nome completo</b>
                            <asp:GridView ID="gdvDependentes" runat="server" AutoGenerateColumns="false" CssClass="tabela" Width="100%"
                                OnRowDataBound="gdvDependentes_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Cartão">
                                        <ItemTemplate>
                                            <asp:Literal ID="litIdBeneficiario" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cod. Dependente">
                                        <ItemTemplate>
                                            <asp:Literal ID="litIdDependente" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Nome Formulário" DataField="Nome" />
                                    <asp:BoundField HeaderText="Data Nascimento" DataField="Nascimento" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField HeaderText="Sexo" DataField="Sexo" />
                                    <asp:TemplateField HeaderText="Parentesco">
                                        <ItemTemplate>
                                            <asp:Literal ID="litParentesco" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Rg" DataField="Rg" />
                                    <asp:BoundField HeaderText="CPF" DataField="Cpf" />
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>

                <h2>Demais Parâmetros</h2>
                <table class="tbBorda" border="1">
                    <tr>
                        <td><b>Plano:</b></td>
                        <td><asp:HiddenField ID="hidPlano" runat="server" /> <asp:Literal ID="litPlano" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><b>Data de Início do Plano:</b></td><td>
                            <asp:TextBox ID="txtDataInicio" runat="server" CssClass="calendario" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td><b>Subcontrato:</b></td><td>
                            <asp:DropDownList ID="dpdCategoria" runat="server" DataValueField="cd_categoria" DataTextField="ds_categoria" OnSelectedIndexChanged="dpdCategoria_SelectedIndexChanged" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td><b>Carência:</b></td><td>
                            <asp:DropDownList ID="dpdCarencia" runat="server">
                                <asp:ListItem Value="" Text="SELECIONE" />
                                <asp:ListItem Value="NOR" Text="Normal" />
                                <asp:ListItem Value="IT" Text="Isento" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td><b>Motivo de bloqueio/desbloqueio (Família):</b></td>
                        <td><asp:DropDownList ID="dpdMotivoDesligamentoFamilia" runat="server" DataValueField="Key" DataTextField="Value" Enabled="false" /><br />
                            <asp:Literal ID="litMensagemPlanoExistenteFamilia" runat="server" Text="Este funcionário já possui família cadastrada no Protheus. Será necessário informar o motivo do bloqueio/desbloqueio de família." Visible="false" />
                            <asp:Literal ID="litMensagemSemPlanoExistenteFamilia" runat="server" Text="Não será necessário informar o motivo de bloqueio/desbloqueio de família." Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td><b>Motivo de bloqueio/desbloqueio (Beneficiário):</b></td>
                        <td><asp:DropDownList ID="dpdMotivoDesligamentoUsuario" runat="server" DataValueField="Key" DataTextField="Value" Enabled="false" /><br />
                            <asp:Literal ID="litMensagemPlanoExistenteUsuario" runat="server" Text="Este funcionário já possui beneficiário cadastrado no Protheus. Será necessário informar o motivo do bloqueio/desbloqueio de beneficiário." Visible="false" />
                            <asp:Literal ID="litMensagemSemPlanoExistenteUsuario" runat="server" Text="Não será necessário informar o motivo de bloqueio/desbloqueio de beneficiário." Visible="false" />
                        </td>
                    </tr>
                </table>
            
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblIntegrada" runat="server" Font-Bold="true" />
                <asp:Button ID="btnIntegrar" runat="server" Text="Realizar Integração" OnClick="btnIntegrar_Click" />
                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClientClick="return goBusca()" />
            </td>
        </tr>
    </table>
</asp:Content>
