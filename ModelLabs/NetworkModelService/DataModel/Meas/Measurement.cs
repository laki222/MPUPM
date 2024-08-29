using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Measurement:IdentifiedObject
    {
        private string measurementType;

        private PhaseCode phases;

        private long psr;

        private long terminal;
        public Measurement(long globalId) : base(globalId)
        {
        }
        public string MeasurementType
        {
            get { return measurementType; }
            set { measurementType = value; }
        }

        public PhaseCode Phases
        {
            get { return phases; }
            set { phases = value; }
        }
        public long PSR
        {
            get { return psr; }
            set { psr = value; }
        }

        public long Terminal
        {
            get { return terminal; }
            set { terminal = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Measurement x = (Measurement)obj;
                return (x.phases == this.phases && x.measurementType == this.measurementType && 
                    x.psr == this.psr && x.terminal == this.terminal);
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
                case ModelCode.MEASUREMENT_PHASES:
                case ModelCode.MEASUREMENT_TYPE:
                case ModelCode.MEASUREMENT_PSR:
                case ModelCode.MEASUREMENT_TERMINAL:
                    return true;

                default:
                    return base.HasProperty(t);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.MEASUREMENT_TYPE:
                    prop.SetValue(measurementType);
                    break;

                case ModelCode.MEASUREMENT_PHASES:
                    prop.SetValue((short)phases);
                    break;
                case ModelCode.MEASUREMENT_PSR:
                    prop.SetValue(psr);
                    break;

                case ModelCode.MEASUREMENT_TERMINAL:
                    prop.SetValue(terminal);
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
                case ModelCode.MEASUREMENT_TYPE:
                    measurementType = property.AsString();
                    break;

                case ModelCode.MEASUREMENT_PHASES:
                    phases = (PhaseCode)property.AsEnum();
                    break;
                case ModelCode.MEASUREMENT_PSR:
                    psr = property.AsReference();
                    break;

                case ModelCode.MEASUREMENT_TERMINAL:
                    terminal = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation
        
        #region IReference implementation
        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (psr != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASUREMENT_PSR] = new List<long>();
                references[ModelCode.MEASUREMENT_PSR].Add(psr);
            }

            if (terminal != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASUREMENT_TERMINAL] = new List<long>();
                references[ModelCode.MEASUREMENT_TERMINAL].Add(terminal);
            }
           
            base.GetReferences(references, refType);
        }

        #endregion IReference implementation
    }
}
