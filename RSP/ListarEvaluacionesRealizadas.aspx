<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ListarEvaluacionesRealizadas.aspx.cs" Inherits="NCCSAN.RSP.ListarEvaluacionesRealizadas" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Popup_background
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        
        .Popup_frontground
        {
            background-color: White;
            padding: 30px; /*border-style: dotted;
            border-width: 2px;*/
            box-shadow: 10px 10px 5px #555555;
        }
    </style>
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Lista de Evaluaciones</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnFiltro" runat="server">
                <table align="center" class="tableControls" style="width: 100%">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="Código o W/O"></asp:Label>
                            <asp:TextBox ID="txtCodigoWO" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="ibSearch" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Icon/search.png"
                                AlternateText="Buscar" ToolTip="Buscar" />
                            <asp:ImageButton ID="ibClearCodigoWO" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Icon/clear.png"
                                OnClick="ibClearCodigoWO_Click" AlternateText="Limpiar" ToolTip="Limpiar" />
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label5" runat="server" Text="Aceptado"></asp:Label>
                            <asp:DropDownList ID="ddlAceptado" runat="server" Width="150px" AutoPostBack="True">
                                <asp:ListItem Selected="True">[Todos]</asp:ListItem>
                                <asp:ListItem>Si</asp:ListItem>
                                <asp:ListItem>No</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Año"></asp:Label>
                            <asp:DropDownList ID="ddlAnio" runat="server" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ddlAnio_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Mes"></asp:Label>
                            <asp:DropDownList ID="ddlMes" runat="server" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="ddlMes_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfFecha" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tableControls" align="center" style="width: 100%">
                <tr>
                    <td class="tdUnique">
                        <asp:GridView ID="gvEvaluaciones" runat="server" AutoGenerateColumns="False" DataKeyNames="codigo"
                            DataSourceID="SDSEvaluaciones" HorizontalAlign="Center" Width="100%" AllowPaging="True"
                            PageSize="20" OnRowCommand="gvEvaluaciones_RowCommand" 
                            ShowHeaderWhenEmpty="True" ondatabound="gvEvaluaciones_DataBound">
                            <AlternatingRowStyle CssClass="rowDataAlt" />
                            <Columns>
                                <asp:BoundField DataField="codigo" HeaderText="Código" ReadOnly="True" SortExpression="codigo">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="work_order" HeaderText="W/O" SortExpression="work_order">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha_evaluacion" HeaderText="Fecha evaluación" SortExpression="fecha_evaluacion">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_centro" HeaderText="Centro" SortExpression="nombre_centro">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_tipoequipo" HeaderText="Equipo" SortExpression="nombre_tipoequipo">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="modelo_equipo" HeaderText="Modelo" SortExpression="modelo_equipo">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_componente" HeaderText="Componente" SortExpression="nombre_componente">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="aceptado" HeaderText="Aceptado" SortExpression="aceptado">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="nombre_origen" HeaderText="Origen falla" SortExpression="nombre_origen">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:ButtonField ButtonType="Image" CommandName="DetalleEvaluacion" HeaderText="Detalle"
                                    ImageUrl="~/Images/Icon/search.png" Text="Detalle">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>
                            </Columns>
                            <HeaderStyle BackColor="#666666" ForeColor="White" />
                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                            <PagerStyle HorizontalAlign="Center" />
                            <RowStyle CssClass="rowData" />
                            <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            <EmptyDataTemplate>
                                <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                    Font-Size="14px"></asp:Label>
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SDSEvaluaciones" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                            
                            
                            SelectCommand="SELECT e.codigo, e.work_order, CONVERT(VARCHAR(10), ev.fecha, 105) AS fecha_evaluacion,  c.nombre AS nombre_centro, e.nombre_tipoequipo, e.modelo_equipo, e.nombre_componente, ev.aceptado, ev.nombre_origen FROM Evento e, Evaluacion ev, Centro c WHERE (ev.codigo_evento=e.codigo) AND (c.id=e.id_centro) AND (e.nombre_cliente=@nombre_cliente) AND (ev.aceptado LIKE '%' + @aceptado + '%') AND (CONVERT(VARCHAR(10), e.fecha_ingreso, 105) LIKE '%' + @fecha + '%') AND ((e.codigo LIKE '%' + @codigowo + '%') OR  (e.work_order LIKE '%' + @codigowo + '%')) ORDER BY e.fecha_ingreso DESC">
                            <SelectParameters>
                                <asp:SessionParameter Name="nombre_cliente" SessionField="nombre_cliente" />
                                <asp:ControlParameter ControlID="ddlAceptado" Name="aceptado" PropertyName="SelectedValue" />
                                <asp:ControlParameter ControlID="hfFecha" ConvertEmptyStringToNull="False" Name="fecha"
                                    PropertyName="Value" Type="String" />
                                <asp:ControlParameter ControlID="txtCodigoWO" ConvertEmptyStringToNull="False" Name="codigowo"
                                    PropertyName="Text" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td class="tdUnique" align="left">
                        <asp:Panel ID="pnOpcionesLista" runat="server" Visible="False">
                            <asp:ImageButton ID="ibExportToExcel" runat="server" ImageUrl="~/Images/Icon/ms_excel.png"
                                OnClick="ibExportToExcel_Click" ToolTip="Descargar como planilla Excel" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ibExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

