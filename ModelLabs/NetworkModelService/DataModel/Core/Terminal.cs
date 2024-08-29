using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Terminal : IdentifiedObject
    {
        private bool connected;

        private PhaseCode phases;

        private int sequenceNumber;

        private long conductingEquipment;

        private long connectivityNode;

        private List<long> measurements = new List<long>();

        public Terminal(long globalId) : base(globalId)
        {
        }
        public bool Connected
        {
            get { return connected; }
            set { connected = value; }
        }

        public PhaseCode Phases
        {
            get { return phases; }
            set { phases = value; }
        }

        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }
        public long ConductingEquipment
        {
            get { return conductingEquipment; }
            set { conductingEquipment = value; }
        }

        public long ConnectivityNode
        {
            get { return connectivityNode; }
            set { connectivityNode = value; }
        }

        public List<long> Measurements
        {
            get { return measurements; }
            set { measurements = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Terminal x = (Terminal)obj;
                return (x.connected == this.connected && x.phases == this.phases && x.sequenceNumber == this.sequenceNumber && 
                    x.conductingEquipment == this.conductingEquipment && x.connectivityNode == this.connectivityNode
                    && CompareHelper.CompareLists(x.measurements, this.measurements));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation

        public override bool HasProperty(ModelCode t)
        {
            switch (t)
            {
                case ModelCode.TERMINAL_CONNECTED:
                case ModelCode.TERMINAL_PHASES:
                case ModelCode.TERMINAL_SEQNUM:
                case ModelCode.TERMINAL_CONDUCTINGEQUIMPMENT:
                case ModelCode.TERMINAL_NODE:
                case ModelCode.TERMINAL_MEASUREMENTS:
                    return true;

                default:
                    return base.HasProperty(t);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.TERMINAL_CONNECTED:
                    prop.SetValue(connected);
                    break;

                case ModelCode.TERMINAL_PHASES:
                    prop.SetValue((short)phases);
                    break;

                case ModelCode.TERMINAL_SEQNUM:
                    prop.SetValue(sequenceNumber);
                    break;
                case ModelCode.TERMINAL_CONDUCTINGEQUIMPMENT:
                    prop.SetValue(conductingEquipment);
                    break;

                case ModelCode.TERMINAL_NODE:
                    prop.SetValue(connectivityNode);
                    break;
                case ModelCode.TERMINAL_MEASUREMENTS:
                    prop.SetValue(measurements);
                    break;

                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONNECTED:
                    connected = property.AsBool();
                    break;

                case ModelCode.TERMINAL_PHASES:
                    phases = (PhaseCode)property.AsEnum();
                    break;

                case ModelCode.TERMINAL_SEQNUM:
                    sequenceNumber = property.AsInt();
                    break;
                case ModelCode.TERMINAL_CONDUCTINGEQUIMPMENT:
                    conductingEquipment = property.AsReference();
                    break;

                case ModelCode.TERMINAL_NODE:
                    connectivityNode = property.AsReference();
                    break;
                 
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation
        public override bool IsReferenced
        {
            get
            {
                return measurements.Count != 0 || base.IsReferenced;
            }
        }
        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (conductingEquipment != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CONDUCTINGEQUIMPMENT] = new List<long>();
                references[ModelCode.TERMINAL_CONDUCTINGEQUIMPMENT].Add(conductingEquipment);
            }

            if (connectivityNode != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_NODE] = new List<long>();
                references[ModelCode.TERMINAL_NODE].Add(connectivityNode);
            }
            if (measurements != null && measurements.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_MEASUREMENTS] = measurements.GetRange(0, measurements.Count);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_TERMINAL:
                    measurements.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MEASUREMENT_TERMINAL:

                    if (measurements.Contains(globalId))
                    {
                        measurements.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }

                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }


        #endregion IReference implementation
    }
}
