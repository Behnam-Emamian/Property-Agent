Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Scheduling
Imports DotNetNuke.Entities.Modules

Namespace Ventrian.PropertyAgent

    Partial Public Class EditExpiryNotifications
        Inherits PropertyAgentBase

#Region " Private Members "


#End Region

#Region " Private Methods "

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim objCrumbMain As New CrumbInfo
            objCrumbMain.Caption = PropertySettings.MainLabel
            objCrumbMain.Url = NavigateURL()
            crumbs.Add(objCrumbMain)

            Dim objCrumbExpiry As New CrumbInfo
            objCrumbExpiry.Caption = Localization.GetString("EditExpiryNotifications", Me.LocalResourceFile)
            objCrumbExpiry.Url = NavigateURL(Me.TabId, "", PropertySettings.SEOAgentType & "=EditExpiryNotifications")
            crumbs.Add(objCrumbExpiry)

            If (PropertySettings.BreadcrumbPlacement = BreadcrumbType.Portal) Then
                For i As Integer = 0 To crumbs.Count - 1
                    Dim objCrumb As CrumbInfo = crumbs(i)
                    If (i > 0) Then
                        Dim objTab As New DotNetNuke.Entities.Tabs.TabInfo
                        objTab.TabID = -8888 + i
                        objTab.TabName = objCrumb.Caption
                        objTab.Url = objCrumb.Url
                        PortalSettings.ActiveTab.BreadCrumbs.Add(objTab)
                    End If
                Next
            End If

            If (PropertySettings.BreadcrumbPlacement = BreadcrumbType.Module) Then
                rptBreadCrumbs.DataSource = crumbs
                rptBreadCrumbs.DataBind()
            End If

        End Sub

        Private Sub BindDefaults()

            txtReminderPeriod.Text = Me.PropertySettings.ReminderPeriod.ToString()
            If Not (drpReminderPeriodMeasurement.Items.FindByValue(Me.PropertySettings.ReminderPeriodMeasurement) Is Nothing) Then
                drpReminderPeriodMeasurement.SelectedValue = Me.PropertySettings.ReminderPeriodMeasurement
            End If

            txtReminderFrequency.Text = Me.PropertySettings.ReminderFrequency.ToString()
            If Not (drpReminderFrequencyMeasurement.Items.FindByValue(Me.PropertySettings.ReminderFrequencyMeasurement) Is Nothing) Then
                drpReminderFrequencyMeasurement.SelectedValue = Me.PropertySettings.ReminderFrequencyMeasurement
            End If

            txtSubject.Text = Me.PropertySettings.ReminderSubject
            txtTemplate.Text = Me.PropertySettings.ReminderTemplate
            txtBCCEmail.Text = Me.PropertySettings.ReminderBCC

            chkEnabled.Checked = Me.PropertySettings.ReminderSchedulerEnabled
            txtTimeLapse.Text = Me.PropertySettings.ReminderSchedulerTimeLapse.ToString()
            If Not (drpTimeLapseMeasurement.Items.FindByValue(Me.PropertySettings.ReminderSchedulerTimeLapseMeasurement) Is Nothing) Then
                drpTimeLapseMeasurement.SelectedValue = Me.PropertySettings.ReminderSchedulerTimeLapseMeasurement
            End If
            txtRetryTimeLapse.Text = Me.PropertySettings.ReminderSchedulerRetryFrequency.ToString()
            If Not (drpRetryTimeLapseMeasurement.Items.FindByValue(Me.PropertySettings.ReminderSchedulerRetryFrequencyMeasurement) Is Nothing) Then
                drpRetryTimeLapseMeasurement.SelectedValue = Me.PropertySettings.ReminderSchedulerRetryFrequencyMeasurement
            End If

        End Sub

        Private Sub BindHistory()

            If (Me.PropertySettings.ReminderScheduleID <> Null.NullInteger) Then

                Dim arrSchedule As ArrayList = SchedulingProvider.Instance.GetScheduleHistory(Me.PropertySettings.ReminderScheduleID)

                If (arrSchedule.Count > 0) Then

                    arrSchedule.Sort(New ScheduleHistorySortStartDate)

                    'Localize Grid
                    Localization.LocalizeDataGrid(dgScheduleHistory, Me.LocalResourceFile)

                    dgScheduleHistory.DataSource = arrSchedule
                    dgScheduleHistory.DataBind()

                    lblNoHistory.Visible = False
                    dgScheduleHistory.Visible = True
                Else
                    lblNoHistory.Visible = True
                    dgScheduleHistory.Visible = False
                End If

            Else

                lblNoHistory.Visible = True
                dgScheduleHistory.Visible = False

            End If

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Try

                BindCrumbs()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                If (IsPostBack = False) Then

                    BindDefaults()
                    BindHistory()

                End If

                cmdUpdate.CssClass = PropertySettings.ButtonClass
                cmdCancel.CssClass = PropertySettings.ButtonClass

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            Try

                If (Page.IsValid) Then

                    Dim objModuleController As New ModuleController

                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_EMAIL_SETTING, PortalSettings.Email)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_PORTAL_NAME_SETTING, PortalSettings.PortalName)

                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_PERIOD_SETTING, txtReminderPeriod.Text)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_PERIOD_MEASUREMENT_SETTING, drpReminderPeriodMeasurement.SelectedValue)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_FREQUENCY_SETTING, txtReminderFrequency.Text)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_FREQUENCY_MEASUREMENT_SETTING, drpReminderFrequencyMeasurement.SelectedValue)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_SUBJECT_SETTING, txtSubject.Text)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_TEMPLATE_SETTING, txtTemplate.Text)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_BCC_SETTING, txtBCCEmail.Text)

                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_SCHEDULER_ENABLED_SETTING, chkEnabled.Checked.ToString())
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_SCHEDULER_TIME_LAPSE_SETTING, txtTimeLapse.Text)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_SCHEDULER_TIME_LAPSE_MEASUREMENT_SETTING, drpTimeLapseMeasurement.SelectedValue)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_SCHEDULER_RETRY_FREQUENCY_SETTING, txtRetryTimeLapse.Text)
                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_SCHEDULER_RETRY_FREQUENCY_MEASUREMENT_SETTING, drpRetryTimeLapseMeasurement.SelectedValue)

                    Dim objScheduleItem As New ScheduleItem

                    objScheduleItem.TypeFullName = "Ventrian.PropertyAgent.ReminderJob, Ventrian.PropertyAgent"

                    If txtTimeLapse.Text = "" Or txtTimeLapse.Text = "0" Or txtTimeLapse.Text = "-1" Then
                        objScheduleItem.TimeLapse = Null.NullInteger
                    Else
                        objScheduleItem.TimeLapse = Convert.ToInt32(txtTimeLapse.Text)
                    End If

                    objScheduleItem.TimeLapseMeasurement = drpTimeLapseMeasurement.SelectedItem.Value

                    If txtRetryTimeLapse.Text = "" Or txtRetryTimeLapse.Text = "0" Or txtRetryTimeLapse.Text = "-1" Then
                        objScheduleItem.RetryTimeLapse = Null.NullInteger
                    Else
                        objScheduleItem.RetryTimeLapse = Convert.ToInt32(txtRetryTimeLapse.Text)
                    End If

                    objScheduleItem.RetryTimeLapseMeasurement = drpRetryTimeLapseMeasurement.SelectedItem.Value
                    objScheduleItem.RetainHistoryNum = 10
                    objScheduleItem.AttachToEvent = ""
                    objScheduleItem.CatchUpEnabled = False
                    objScheduleItem.Enabled = chkEnabled.Checked
                    objScheduleItem.ObjectDependencies = ""
                    objScheduleItem.Servers = ""

                    If (Me.PropertySettings.ReminderScheduleID <> Null.NullInteger) Then

                        If (SchedulingProvider.Instance().GetSchedule(Me.PropertySettings.ReminderScheduleID) Is Nothing) Then
                            objScheduleItem.ScheduleID = SchedulingProvider.Instance().AddSchedule(objScheduleItem)
                        Else
                            objScheduleItem.ScheduleID = Me.PropertySettings.ReminderScheduleID
                            SchedulingProvider.Instance().UpdateSchedule(objScheduleItem)
                        End If

                    Else

                        objScheduleItem.ScheduleID = SchedulingProvider.Instance().AddSchedule(objScheduleItem)

                    End If

                    SchedulingProvider.Instance().AddScheduleItemSetting(objScheduleItem.ScheduleID, "PortalID", Me.PortalSettings.PortalId.ToString())
                    SchedulingProvider.Instance().AddScheduleItemSetting(objScheduleItem.ScheduleID, "ModuleID", Me.ModuleId.ToString())
                    SchedulingProvider.Instance().AddScheduleItemSetting(objScheduleItem.ScheduleID, "TabID", Me.TabId.ToString())

                    SchedulingProvider.Instance().AddScheduleItemSetting(objScheduleItem.ScheduleID, "Period", txtReminderPeriod.Text)
                    SchedulingProvider.Instance().AddScheduleItemSetting(objScheduleItem.ScheduleID, "PeriodMeasurement", drpReminderPeriodMeasurement.SelectedValue)

                    SchedulingProvider.Instance().AddScheduleItemSetting(objScheduleItem.ScheduleID, "Frequency", txtReminderFrequency.Text)
                    SchedulingProvider.Instance().AddScheduleItemSetting(objScheduleItem.ScheduleID, "FrequencyMeasurement", drpReminderFrequencyMeasurement.SelectedValue)

                    objModuleController.UpdateModuleSetting(Me.ModuleId, Constants.REMINDER_SCHEDULER_ID_SETTING, objScheduleItem.ScheduleID.ToString())

                    Response.Redirect(NavigateURL(), True)

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try


        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click

            Try

                Response.Redirect(NavigateURL(), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

    End Class

End Namespace