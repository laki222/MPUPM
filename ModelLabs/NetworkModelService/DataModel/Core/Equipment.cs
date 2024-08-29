using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Meas;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class Equipment : PowerSystemResource
	{		
		private bool aggregate;
		private bool normallyInService;
        private long equipmentContainer;

        public Equipment(long globalId) : base(globalId) 
		{
		}

        public long EquipmentContainer
        {
            get { return equipmentContainer; }
            set { equipmentContainer = value; }
        }


        public bool Aggregate
        {
			get
			{
				return aggregate;
			}

			set
			{
                aggregate = value;
			}
		}

		public bool NormallyInService
        {
			get 
			{
				return normallyInService; 
			}
			
			set
			{
                normallyInService = value; 
			}
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				Equipment x = (Equipment)obj;
				return (x.equipmentContainer == this.equipmentContainer && (x.aggregate == this.aggregate) &&
						(x.normallyInService == this.normallyInService));
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

		public override bool HasProperty(ModelCode property)
		{
			switch (property)
			{
				case ModelCode.EQUIPMENT_AGGREGATE:
				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
				case ModelCode.EQUIPMENT_CONTAINER:
		
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.EQUIPMENT_AGGREGATE:
					property.SetValue(aggregate);
					break;

				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
					property.SetValue(normallyInService);
					break;
                case ModelCode.EQUIPMENT_CONTAINER:
                    property.SetValue(equipmentContainer);
                    break;

                default:
					base.GetProperty(property);
					break;
			}
		}

		public override void SetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.EQUIPMENT_AGGREGATE:
                    aggregate = property.AsBool();
					break;

				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
                    normallyInService = property.AsBool();
					break;
                case ModelCode.EQUIPMENT_CONTAINER:
                    equipmentContainer = property.AsReference();
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
            if (equipmentContainer != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.EQUIPMENT_CONTAINER] = new List<long>();
                references[ModelCode.EQUIPMENT_CONTAINER].Add(equipmentContainer);
            }

            base.GetReferences(references, refType);
        }
        #endregion IReference implementation
    }
}
