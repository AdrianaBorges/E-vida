<%@ Page Title="Autorização de Reciprocidade" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormReciprocidade.aspx.cs" Inherits="eVidaIntranet.Forms.FormReciprocidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
<div id="conteudo" style="margin-left:15px;" runat="server">
    <div style="text-align:center">
        <asp:ImageButton ID="btnPrint" runat="server" ImageUrl="~/img/print.png" Visible="false" Width="50px" />
        <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Visible="false" Width="50px" />
    </div>
    <table width="100%">
        <tr>
            <td align="center">
                <img src="../img/logo-sized.jpg" alt="Evida" />
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" cellpadding="10px" cellspacing="10px">
                    <tr>
                        <td><b>E-VIDA</b><br />
                            Caixa de Assistência do Setor Elétrico
                        </td>
                        <td>704/705 Norte Bloco C Loja 48<br />
                            70.730-630 Brasília DF</td>
                        <td >
                            Tel.: 0800-607-8300<br />
                            www.e-vida.org.br

                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td><hr /></td>
        </tr>
        <tr>
            <td>
                <table width="100%">
                    <tr>
                        <td style="width:60%"><b>Para:</b>
                            <asp:DropDownList ID="dpdEmpresa" runat="server" Width="400px" 
                                OnSelectedIndexChanged="dpdEmpresa_SelectedIndexChanged" AutoPostBack="true" />
                        </td>
                        <td>Tel nº: <asp:Label ID="lblTelefone" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><b>De:</b> E-VIDA</td>
                        <td>Fax nº: <asp:Label ID="lblFax" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><b>Ref.:</b> Autorização</td>
                        <td>Data: <asp:Label ID="lblData" runat="server" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td><hr /></td>
        </tr>
        <tr>
            <td>
                CONSIDERANDO OS TERMOS DO CONVÊNIO DE RECIPROCIDADE FIRMADO ENTRE A E-VIDA E ESSA INSTITUIÇÃO, SOLICITAMOS AUTORIZAÇÃO DE ATENDIMENTO ASSISTENCIAL, COM EMISSÃO DO CARTÃO DE IDENTIFICAÇÃO, PARA <b>OS (AS) BENEFICIARIOS (AS)</b> CONFORME DADOS ABAIXO:
            </td>
        </tr>
        <tr>
            <td><hr /></td>
        </tr>
        <tr>
            <td>
                <b>TITULAR:</b> <asp:Label ID="lblTitular" runat="server" /><br />
                <b>CARTÃO: </b>
                <asp:Label ID="lblCartao" runat="server" /><br />
                <b>DATA NASC: </b>
                <asp:Label ID="lblNascimento" runat="server" /><br />
                <b>CPF: </b>
                <asp:Label ID="lblCpf" runat="server" /><br />
                <b>CNS: </b>
                <asp:Label ID="lblCns" runat="server" /><br />
                <b>ESTADO CIVIL: </b>
                <asp:Label ID="lblEstadoCivil" runat="server" /><br />
                <b>GENITORA: </b>
                <asp:Label ID="lblNomeMae" runat="server" /><br /><br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:DataList ID="dlBeneficiarios" runat="server" 
                    OnItemDataBound="dlBeneficiarios_ItemDataBound">
                    <ItemTemplate>
                        <b>DEPENDENTE:</b>
                        <asp:Label ID="lblNome" runat="server"/><br />
                        <b>DATA NASCIMENTO:</b>
                        <asp:Label ID="lblNascimento" runat="server"/><br />
                        <b>GRAU DE PARENTESCO:</b>
                        <asp:Label ID="lblParentesco" runat="server"/><br />
                        <b>CPF: </b>
                        <asp:Label ID="lblCpf" runat="server" /><br />
                        <b>CNS: </b>
                        <asp:Label ID="lblCns" runat="server" /><br />
                        <b>GENITORA:</b>
                        <asp:Label ID="lblNomeMae" runat="server"/><br /><br />
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
        <tr>
            <td>
                <b>DATA DE VALIDADE: </b><asp:Label ID="lblDataInicio" runat="server"/> a <asp:Label ID="lblDataFim" runat="server" /><br />
                <b>LOCAL DE ATENDIMENTO: </b><asp:Label ID="lblMunicipio" runat="server" /> - <asp:Label ID="lblUf" runat="server" />
            </td>
        </tr>
        <tr><td><br /></td></tr>
        <tr>
            <td>
                <b>ASSISTÊNCIAS AUTORIZADAS:</b><br />
                <asp:CheckBoxList ID="chkAssistencias" runat="server">
                    <asp:ListItem Value="1" Text="CONSULTAS"/>
                    <asp:ListItem Value="2" Text="MEDICO-HOSPITALAR"/>
                    <asp:ListItem Value="3" Text="EXAMES COMPLEMENTARES"/>
                    <asp:ListItem Value="4" Text="INTERNAÇÕES E CIRURGIAS"/>
                </asp:CheckBoxList>
            </td>
        </tr>
        <tr><td><br /></td></tr>
        <tr>
            <td>
                <b>OBSERVAÇÃO(ÕES): </b><br />
                <asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine" MaxLength="200" Width="100%" Rows="3" />
            </td>
        </tr>
        <tr>
            <td align="center">            
ATENCIOSAMENTE<br /><br /><br />

                <asp:Image ID="imgAssinatura" runat="server" Height="50px" /><br />

<b><asp:Literal ID="litUsuario" runat="server" /></b><br />
<asp:Literal ID="litCargo" runat="server" /><br />
<span style="color: red"><b> PLANTÃO SOCIAL: 61-9968-0322</b></span>

            </td>
        </tr>
        <tr>
            <td align="center"><asp:Button ID="btnSalvar" runat="server" Text="Enviar Autorização" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
        
</div>
</asp:Content>
