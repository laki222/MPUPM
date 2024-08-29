using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	/// <summary>
	/// PowerTransformerImporter
	/// </summary>
	public class PowerTransformerImporter
	{
		/// <summary> Singleton </summary>
		private static PowerTransformerImporter ptImporter = null;
		private static object singletoneLock = new object();

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;


		#region Properties
		public static PowerTransformerImporter Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletoneLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new PowerTransformerImporter();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get 
			{
				return delta;
			}
		}
		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
		{
			LogManager.Log("Importing PowerTransformer Elements...", LogLevel.Info);
			report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if ((concreteModel != null) && (concreteModel.ModelMap != null))
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta();
				}
				catch (Exception ex)
				{
					string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}
			LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
			return report;
		}

		/// <summary>
		/// Method performs conversion of network elements from CIM based concrete model into DMS model.
		/// </summary>
		private void ConvertModelAndPopulateDelta()
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            //// import all concrete model types (DMSType enum)
            ImportEquipmentContainers();
            ImportConnectivityNodes();
			ImportSwitchs();
			ImportTerminals();
			ImportMeasurements();

			LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

		#region Import
		private void ImportEquipmentContainers()
		{
			SortedDictionary<string, object> cimEquipmentContainers = concreteModel.GetAllObjectsOfType("FTN.EquipmentContainer");
			if (cimEquipmentContainers != null)
			{
				foreach (KeyValuePair<string, object> cimEquipmentContainerPair in cimEquipmentContainers)
				{
					FTN.EquipmentContainer cimEquipmentContainer = cimEquipmentContainerPair.Value as FTN.EquipmentContainer;

					ResourceDescription rd = CreateEquipmentContainerResourceDescription(cimEquipmentContainer);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("EquipmentContainer ID = ").Append(cimEquipmentContainer.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("EquipmentContainer ID = ").Append(cimEquipmentContainer.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateEquipmentContainerResourceDescription(FTN.EquipmentContainer cimEquipmentContainer)
		{
			ResourceDescription rd = null;
			if (cimEquipmentContainer != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.EQUIPMENTCONTAINER, importHelper.CheckOutIndexForDMSType(DMSType.EQUIPMENTCONTAINER));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimEquipmentContainer.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateEquipmentContainerProperties(cimEquipmentContainer, rd,importHelper,report);
			}
			return rd;
		}
		
		private void ImportConnectivityNodes()
		{
			SortedDictionary<string, object> cimConnectivityNodes = concreteModel.GetAllObjectsOfType("FTN.ConnectivityNode");
			if (cimConnectivityNodes != null)
			{
				foreach (KeyValuePair<string, object> cimConnectivityNodePair in cimConnectivityNodes)
				{
					FTN.ConnectivityNode cimConnectivityNode = cimConnectivityNodePair.Value as FTN.ConnectivityNode;

					ResourceDescription rd = CreateConnectivityNodeResourceDescription(cimConnectivityNode);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("ConnectivityNode ID = ").Append(cimConnectivityNode.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("ConnectivityNode ID = ").Append(cimConnectivityNode.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateConnectivityNodeResourceDescription(FTN.ConnectivityNode cimConnectivityNode)
		{
			ResourceDescription rd = null;
			if (cimConnectivityNode != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONNECTIVITYNODE, importHelper.CheckOutIndexForDMSType(DMSType.CONNECTIVITYNODE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimConnectivityNode.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateConnectivityNodeProperties(cimConnectivityNode, rd,importHelper,report);
			}
			return rd;
		}

		private void ImportSwitchs()
		{
			SortedDictionary<string, object> cimSwitchs = concreteModel.GetAllObjectsOfType("FTN.Switch");
			if (cimSwitchs != null)
			{
				foreach (KeyValuePair<string, object> cimPowerTransformerPair in cimSwitchs)
				{
					FTN.Switch cimSwitch = cimPowerTransformerPair.Value as FTN.Switch;

					ResourceDescription rd = CreateSwitchResourceDescription(cimSwitch);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("Switch ID = ").Append(cimSwitch.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("Switch ID = ").Append(cimSwitch.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateSwitchResourceDescription(FTN.Switch cimSwitch)
		{
			ResourceDescription rd = null;
			if (cimSwitch != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SWITCH, importHelper.CheckOutIndexForDMSType(DMSType.SWITCH));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimSwitch.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateSwitchProperties(cimSwitch, rd, importHelper, report);
			}
			return rd;
		}

        private void ImportTerminals()
        {
            SortedDictionary<string, object> cimTerminals = concreteModel.GetAllObjectsOfType("FTN.Terminal");
            if (cimTerminals != null)
            {
                foreach (KeyValuePair<string, object> cimTerminalPair in cimTerminals)
                {
                    FTN.Terminal cimTerminal = cimTerminalPair.Value as FTN.Terminal;

                    ResourceDescription rd = CreateTerminalResourceDescription(cimTerminal);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateTerminalResourceDescription(FTN.Terminal cimTerminal)
        {
            ResourceDescription rd = null;
            if (cimTerminal != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TERMINAL, importHelper.CheckOutIndexForDMSType(DMSType.TERMINAL));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTerminal.ID, gid);

                ////populate ResourceDescription
               PowerTransformerConverter.PopulateTerminalProperties(cimTerminal, rd, importHelper, report);
            }
            return rd;
        }
        private void ImportMeasurements()
		{
			SortedDictionary<string, object> cimMeasurements = concreteModel.GetAllObjectsOfType("FTN.Measurement");
			if (cimMeasurements != null)
			{
				foreach (KeyValuePair<string, object> cimWindingTestPair in cimMeasurements)
				{
					FTN.Measurement cimMeasurement = cimWindingTestPair.Value as FTN.Measurement;

					ResourceDescription rd = CreateMeasurementResourceDescription(cimMeasurement);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("Measurement ID = ").Append(cimMeasurement.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("Measurement ID = ").Append(cimMeasurement.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateMeasurementResourceDescription(FTN.Measurement cimMeasurement)
		{
			ResourceDescription rd = null;
			if (cimMeasurement != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.MEASUREMENT, importHelper.CheckOutIndexForDMSType(DMSType.MEASUREMENT));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimMeasurement.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateMeasurementProperties(cimMeasurement, rd, importHelper, report);
			}
			return rd;
		}
		#endregion Import
	}
}

