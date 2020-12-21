<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Emails.aspx.cs" Inherits="NCCSAN.Administracion.Emails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Configuración de Emails</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnResumenes" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Resumenes</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <p>
                                En esta tabla se especifica a partir de cúantos días antes de finalizar el mes se
                                comienza el envío de correos de alerta indicando el resumen de tareas pendientes
                                en el centro requeridas para cerrar el mes en curso.
                                <br />
                                El resumen generado indica lo siguiente:
                                <ul>
                                    <li>Cantidad de Acciones Inmediatas</li>
                                    <li>Cantidad de Planes de Acción, con 100% de progreso que no se han cerrado</li>
                                    <li>Cantidad de Verificaciones de Plan de Acción</li>
                                </ul>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Table ID="tbResumenes" runat="server" Width="100%">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell Text="Caso" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Receptor" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Días" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Activado" BackColor="#666666" ForeColor="White" />
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label1" runat="server" Text="Tareas pendientes en el centro"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label2" runat="server" Text="Jefe de Calidad"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtResumenJefeCalidadDias" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbResumenJefeCalidadActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label3" runat="server" Text="Tareas pendientes en el centro"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label4" runat="server" Text="Coordinador"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtResumenCoordinadorDias" runat="server" TextMode="Number" MaxLength="2"
                                            Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbResumenCoordinadorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <br />
            <br />
            <asp:Panel ID="pnAccionesInmediatas" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Acciones Inmediatas</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <p>
                                En esta tabla se especifica a partir de cúantos días luego de registrado un Evento con bajo IRC (que requiere registrar Acción Inmediata)
                                se contabiliza dentro del resumen de tareas pendientes enviado al Inspector.
                                <br />
                                Es importante destacar que se contabilizan solo las Acciones Inmediatas que no fueron registradas al momento de registrar el Evento por parte del Inspector (Evento con estado "Acción Inmediata pendiente").
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Table ID="tbAccionesInmediatas" runat="server" Width="100%">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell Text="Estado" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Receptor" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Días alerta" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Días límite" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Activado" BackColor="#666666" ForeColor="White" />
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label33" runat="server" Text="Acción Inmediata pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label34" runat="server" Text="Inspector"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtAccionInmediataPendienteInspectorDiasAlerta" runat="server" TextMode="Number" MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtAccionInmediataPendienteInspectorDiasLimite" runat="server" TextMode="Number" MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbAccionInmediataPendienteInspectorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <br />
            <br />
            <asp:Panel ID="pnInvestigaciones" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Investigaciones</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <p>
                                En esta tabla se especifica a partir de cúantos días luego de cambiado el estado
                                del Evento (indicado en la columna "Estado") comienza el envío de correos de alerta
                                indicando la información de la tarea pendiente (desde "Días alerta" hasta "Días
                                límite").
                                <br />
                                Es importante destacar que al ser Investigaciones se tratan individualmente, es
                                decir, por cada Evento que esté en el estado indicado se enviará un correo al responsable
                                seleccionado (indicado en la columna "Responsable").
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Table ID="tbInvestigaciones" runat="server" Width="100%">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell Text="Estado" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Receptor" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Días alerta" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Días límite" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Activado" BackColor="#666666" ForeColor="White" />
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label7" runat="server" Text="Investigación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label8" runat="server" Text="Jefe de Calidad"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionPendienteJefeCalidadDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionPendienteJefeCalidadDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbInvestigacionPendienteJefeCalidadActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label9" runat="server" Text="Investigación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label10" runat="server" Text="Coordinador"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionPendienteCoordinadorDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionPendienteCoordinadorDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbInvestigacionPendienteCoordinadorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label5" runat="server" Text="Investigación en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label6" runat="server" Text="Jefe de Calidad"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionEnCursoJefeCalidadDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionEnCursoJefeCalidadDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbInvestigacionEnCursoJefeCalidadActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label11" runat="server" Text="Investigación en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label12" runat="server" Text="Coordinador"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionEnCursoCoordinadorDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionEnCursoCoordinadorDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbInvestigacionEnCursoCoordinadorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label13" runat="server" Text="Investigación en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label14" runat="server" Text="Responsable"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionEnCursoResponsableDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtInvestigacionEnCursoResponsableDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbInvestigacionEnCursoResponsableActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label15" runat="server" Text="Evaluación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label16" runat="server" Text="Jefe de Calidad"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtEvaluacionPendienteJefeCalidadDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtEvaluacionPendienteJefeCalidadDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbEvaluacionPendienteJefeCalidadActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label17" runat="server" Text="Evaluación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label18" runat="server" Text="Coordinador"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtEvaluacionPendienteCoordinadorDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtEvaluacionPendienteCoordinadorDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbEvaluacionPendienteCoordinadorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <br />
            <br />
            <asp:Panel ID="pnPlanesAccion" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Planes de Acción</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <p>
                                En esta tabla se especifica a partir de cúantos días luego de cambiado el estado
                                del Evento a "Plan de acción pendiente" comienza el envío de correos de alerta indicando
                                la información de la tarea pendiente (desde "Días alerta" hasta "Días límite").
                                <br />
                                En el caso de las Acciones Correctivas sólo se indica a partir de cuántos días antes
                                de alcanzar la fecha límite comienza el envío de correos de alerta hasta el día
                                de su vencimiento. Es importante destacar que al ser Acciones Correctivas se tratan
                                individualmente, es decir, por cada Acción Correctiva en curso se enviará un correo
                                al responsable seleccionado (indicado en la columna "Responsable").
                                <br />
                                Para las Verificaciones pendientes sólo se indica a partir de cúantos días luego
                                de cerrar el Plan de Acción se contabiliza dentro del resumen enviado (campo "Días
                                alerta").
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Table ID="tbPlanesAccion" runat="server" Width="100%">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell Text="Estado" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Receptor" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Días alerta" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Días límite" BackColor="#666666" ForeColor="White" />
                                    <asp:TableHeaderCell Text="Activado" BackColor="#666666" ForeColor="White" />
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label19" runat="server" Text="Plan de acción pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label20" runat="server" Text="Jefe de Calidad"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtPlanAccionPendienteJefeCalidadDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtPlanAccionPendienteJefeCalidadDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbPlanAccionPendienteJefeCalidadActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label21" runat="server" Text="Plan de acción pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label22" runat="server" Text="Coordinador"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtPlanAccionPendienteCoordinadorDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtPlanAccionPendienteCoordinadorDiasLimite" runat="server" TextMode="Number"
                                            MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbPlanAccionPendienteCoordinadorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label23" runat="server" Text="Acción correctiva en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label24" runat="server" Text="Jefe de Calidad"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtAccionCorrectivaEnCursoJefeCalidadDiasAlerta" runat="server"
                                            TextMode="Number" MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">

                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbAccionCorrectivaEnCursoJefeCalidadActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label25" runat="server" Text="Acción correctiva en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label26" runat="server" Text="Coordinador"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtAccionCorrectivaEnCursoCoordinadorDiasAlerta" runat="server"
                                            TextMode="Number" MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">

                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbAccionCorrectivaEnCursoCoordinadorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label27" runat="server" Text="Acción correctiva en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label28" runat="server" Text="Responsable"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtAccionCorrectivaEnCursoResponsableDiasAlerta" runat="server"
                                            TextMode="Number" MaxLength="2" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">

                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbAccionCorrectivaEnCursoResponsableActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label29" runat="server" Text="Verificación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label30" runat="server" Text="Jefe de Calidad"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtVerificacionPendienteJefeCalidadDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="3" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbVerificacionPendienteJefeCalidadActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label31" runat="server" Text="Verificación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label32" runat="server" Text="Coordinador"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:TextBox ID="txtVerificacionPendienteCoordinadorDiasAlerta" runat="server" TextMode="Number"
                                            MaxLength="3" Width="30px"></asp:TextBox>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:CheckBox ID="chbVerificacionPendienteCoordinadorActivado" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <br />
            <table align="center">
                <tr>
                    <td>
                        <asp:ImageButton ID="ibGuardarConfiguracionEmails" runat="server" CssClass="submitPanelButton"
                            ImageUrl="~/Images/Button/save.png" OnClick="ibGuardarConfiguracionEmails_Click" />
                    </td>
                </tr>
            </table>
            <div align="left" style="color: #FF0000; text-align: center">
                <asp:Literal ID="ltSummary" runat="server"></asp:Literal>
            </div>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
