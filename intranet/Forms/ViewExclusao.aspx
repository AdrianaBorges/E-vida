<%@ Page Title="Visualização da Solicitação de Exclusão de Beneficiário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ViewExclusao.aspx.cs" Inherits="eVidaIntranet.Forms.ViewExclusao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function openPdf() {
    		openReport(RELATORIO_EXCLUSAO, 'ID=<%= Request["ID"] %>');
    		return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo" style="margin-left: 15px">
        <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
                <td rowspan="2" valign="top">
                    <asp:Label ID="lblMatricula" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:Label ID="txtMatricula" runat="server" Width="150px"/>
                </td>
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:Label ID="txtEmailTitular" runat="server" Width="400px" />
                </td>
                <td  valign="top">
                    <asp:Label ID="lblOrotocolo" runat="server" Text=" Protocolo" Font-Bold="true" /><br />
                    <asp:Label ID="txtProtocolo" runat="server" Width="150px"/>
                </td>

            </tr>
        </table>
        <br />
        

        <h2>2 - Beneficiários para Exclusão</h2>
	    <asp:GridView ID="gdvDependentes" runat="server" Width="100%" AutoGenerateColumns="false"
            DataKeyNames="Cdusuario" OnRowDataBound="gdvDependentes_RowDataBound" >
            <AlternatingRowStyle CssClass="tbDependenteAlt" />
            <RowStyle CssClass="tbDependente" BorderWidth="1px" />
            
            <Columns>
                <asp:BoundField HeaderText="Seq." />
                <asp:BoundField HeaderText="Nome Beneficiário" DataField="Nome" />
                <asp:BoundField HeaderText="Parentesco" DataField="Parentesco" />
                <asp:BoundField HeaderText="Plano Vinculado" DataField="Plano" />
            </Columns>
        </asp:GridView>

        <h2>3 - DECLARACAO</h2>
	    <div class="observacao">
		    <p>Compete ao beneficiário titular a devolução de sua carteira de identificação no momento do seu desligamento do plano, 
				bem como a devolução das carteiras de identificação dos seus dependentes vinculados aos planos de saúde quando perderem essa condição.</p>
            <p>Em caso de uso indevido e da não devolução das carteiras de identificação, declaro que estou ciente que participarei 
				integralmente nas despesas referentes à utilização do plano de saúde.
            </p>
			<p>O desligamento do titular ou de seus dependentes não exime o beneficiário titular de quitar eventuais débitos com a 
				E-VIDA, incluindo os valores de contribuição mensal e de co-participação.
			</p>
	    </div>

	    <h2>4 - CONSEQUÊNCIAS DO CANCELAMENTO OU EXCLUSÃO DO CONTRATO DE PLANO DE SAÚDE</h2>
	    <div class="observacao">
		    <p>Resolução Normativa nº 412, de 10 de novembro de 2016, da ANS.</p>
			<p>Art. 15. Recebida pela operadora ou administradora de benefícios, a solicitação do cancelamento do contrato de plano de saúde individual ou familiar ou de exclusão de beneficiários em plano coletivo empresarial ou coletivo por adesão, a operadora ou administradora de benefícios, destinatária do pedido, deverá prestar de forma clara e precisa, no mínimo, as seguintes informações:</p>
		    <p>I – eventual ingresso em novo plano de saúde poderá importar:</p>
		    <p>a) no cumprimento de novos períodos de carência, observado o disposto no inciso V do artigo 12, da Lei nº 9.656, de 3 de junho de 1998;</p>
		    <p>b) na perda do direito à portabilidade de carências, caso não tenha sido este o motivo do pedido, nos termos previstos na RN nº 186, de 14 de janeiro de 2009, que dispõe, em especial, sobre a regulamentação da portabilidade das carências previstas no inciso V do art. 12 da Lei nº 9.656, de 3 de junho de 1998;</p>
		    <p>c) no preenchimento de nova declaração de saúde, e, caso haja doença ou lesão preexistente – DLP, no cumprimento de Cobertura Parcial Temporária – CPT, que determina, por um período ininterrupto de até 24 meses, a partir da data da contratação ou adesão ao novo plano, a suspensão da cobertura de Procedimentos de Alta Complexidade (PAC), leitos de alta tecnologia e procedimentos cirúrgicos;</p>
		    <p>d) na perda imediata do direito de remissão, quando houver, devendo o beneficiário arcar com o pagamento de um novo contrato de plano de saúde que venha a contratar;</p>
		    <p>II - efeito imediato e caráter irrevogável da solicitação de cancelamento do contrato ou exclusão de beneficiário, a partir da ciência da operadora ou administradora de benefícios;</p>
		    <p>III – as contraprestações pecuniárias vencidas e/ou eventuais coparticipações devidas, nos planos em pré-pagamento ou em pós-pagamento, pela utilização de serviços realizados antes da solicitação de cancelamento ou exclusão do plano de saúde são de responsabilidade do beneficiário;</p>
		    <p>IV - as despesas decorrentes de eventuais utilizações dos serviços pelos beneficiários após a data de solicitação de cancelamento ou exclusão do plano de saúde, inclusive nos casos de urgência ou emergência, correrão por sua conta;</p>
		    <p>V – a exclusão do beneficiário titular do contrato individual ou familiar não extingue o contrato, sendo assegurado aos dependentes já inscritos o direito à manutenção das mesmas condições contratuais, com a assunção das obrigações decorrentes; e</p>
		    <p>VI – a exclusão do beneficiário titular do contrato coletivo empresarial ou por adesão observará as disposições contratuais quanto à exclusão ou não dos dependentes, conforme o disposto no inciso II do parágrafo único do artigo 18, da RN nº 195, de 14 de julho de 2009, que dispõe sobre a classificação e características dos planos privados de assistência à saúde, regulamenta a sua contratação, institui a orientação para contratação de planos privados de assistência à saúde e dá outras providências.</p>
	    </div>
        
	    <div>
		    <br />
		    Declaro, para os devidos fins, que tenho plena ciência das consequências do cancelamento ou exclusão do contrato de plano de saúde.
		    <br />
	    </div>

        <div>
            <br />
	        (Local e Data) <asp:Label ID="txtLocal" runat="server" Width="150px" CssClass="inputInline" />, <asp:Label ID="lblData" runat="server" />
	        <br />            
	    </div>

        <div>
            <table width="100%">
                <tr>                    
                    <td align="center"><asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClientClick="history.back(-1)" /></td>
                    <td align="center"><asp:ImageButton ID="btnPdf" ImageUrl="../img/pdf.png" runat="server" ToolTip="Gerar PDF" Width="50px" CssClass="printer" OnClientClick="openPdf()" /></td>
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>
