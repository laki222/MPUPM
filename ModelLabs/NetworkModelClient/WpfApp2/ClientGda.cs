using FTN.Common;
using FTN.ServiceContracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModelClient
{
    public class ClientGDA : IDisposable
    {
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
        private NetworkModelGDAProxy gdaQueryProxy = null;

        public ClientGDA()
        {

        }

        private NetworkModelGDAProxy GdaQueryProxy
        {
            get
            {
                if (gdaQueryProxy != null)
                {
                    gdaQueryProxy.Abort();
                    gdaQueryProxy = null;
                }

                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaQueryProxy.Open();

                return gdaQueryProxy;
            }
        }

        #region GDAQueryService


        public List<long> GetAllGids()
        {
            ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
            List<ModelCode> properties = new List<ModelCode>();
            List<long> ids = new List<long>();

            int iteratorId = 0;
            int numberOfResources = 1000;
            DMSType currType = 0;
            properties.Add(ModelCode.IDOBJ_GID);
            try
            {
                foreach (DMSType type in Enum.GetValues(typeof(DMSType)))
                {
                    currType = type;

                    if (type != DMSType.MASK_TYPE)
                    {
                        iteratorId = GdaQueryProxy.GetExtentValues(modelResourcesDesc.GetModelCodeFromType(type), properties);
                        int count = GdaQueryProxy.IteratorResourcesLeft(iteratorId);

                        while (count > 0)
                        {
                            List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                            for (int i = 0; i < rds.Count; i++)
                            {
                                ids.Add(rds[i].Id);
                            }

                            count = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                        }

                        bool ok = GdaQueryProxy.IteratorClose(iteratorId);

                    }
                }
            }

            catch (Exception)
            {
                throw;
            }

            return ids;
        }


        public List<ModelCode> GetAllPropertyIdsForEntityId(long globalId)
        {
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
            return modelResourcesDesc.GetAllPropertyIds(type);
        }

        public string GetValues(long globalId, List<ModelCode> props)
        {
            ResourceDescription rd = null;
            string ss = "";
            List<ModelCode> properties = new List<ModelCode>();
            try
            {
                short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
                properties = props;
                // modelResourcesDesc.GetAllPropertyIds((DMSType)type);

                rd = gdaQueryProxy.GetValues(globalId, properties);
                ss += String.Format("Item with gid: 0x{0:x16}:\n", globalId);
                foreach (Property p in rd.Properties)
                {
                    ss += String.Format("{0} =", p.Id);
                    switch (p.Type)
                    {
                        case PropertyType.Float:
                            ss += String.Format(" {0}:\n", p.AsFloat());
                            break;
                        case PropertyType.Enum:
                            try
                            {
                                EnumDescs enumDescs = new EnumDescs();
                                ss += String.Format(" {0}\n", enumDescs.GetStringFromEnum(p.Id, p.AsEnum()));
                            }
                            catch
                            {
                                ss += String.Format(" {0}\n", p.AsEnum().ToString());
                            }
                            break;
                        case PropertyType.Bool:
                            if (p.Id == ModelCode.IDOBJ_GID)
                            {
                                ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                            }
                            else
                            {
                                ss += String.Format(" {0}\n", p.AsBool());
                            }

                            break;
                        case PropertyType.Int32:
                            if (p.Id == ModelCode.IDOBJ_GID)
                            {
                                ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                            }
                            else
                            {
                                ss += String.Format(" {0}\n", p.AsInt());
                            }

                            break;
                        case PropertyType.Int64:
                            if (p.Id == ModelCode.IDOBJ_GID)
                            {
                                ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                            }
                            else
                            {
                                ss += String.Format(" {0}\n", p.AsInt());
                            }

                            break;
                        case PropertyType.DateTime:
                            if (p.Id == ModelCode.IDOBJ_GID)
                            {
                                ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                            }
                            else
                            {
                                ss += String.Format(" {0}\n", p.AsDateTime());
                            }

                            break;

                        case PropertyType.Reference:
                            ss += (String.Format(" 0x{0:x16}\n", p.AsReference()));
                            break;
                        case PropertyType.String:
                            if (p.PropertyValue.StringValue == null)
                            {
                                p.PropertyValue.StringValue = String.Empty;
                            }
                            ss += String.Format(" {0}\n", p.AsString());
                            break;


                        case PropertyType.ReferenceVector:
                            if (p.AsLongs().Count > 0)
                            {
                                string s = "";
                                for (int j = 0; j < p.AsLongs().Count; j++)
                                {
                                    s += (String.Format(" 0x{0:x16},\n", p.AsLongs()[j]));
                                }

                                ss += s;// (sb.ToString(0, sb.Length - 2));
                            }
                            else
                            {
                                ss += ("empty long/reference vector\n");
                            }

                            break;


                        default:
                            throw new Exception("Failed to export Resource Description as XML. Invalid property type.");
                    }
                }
            }
            catch (Exception)
            {

            }

            return ss;
        }

        public string GetRelatedValues(long sourceGlobalId, Association association, List<ModelCode> props)
        {

            string ss = "";
            int numberOfResources = 2;
            bool gidBool = true;
            try
            {
                List<ModelCode> properties = props;
                if (props.Contains(ModelCode.IDOBJ_GID) == false)
                {
                    properties.Add(ModelCode.IDOBJ_GID);
                    gidBool = false;
                }
                int iteratorId = gdaQueryProxy.GetRelatedValues(sourceGlobalId, properties, association);
                int resourcesLeft = gdaQueryProxy.IteratorResourcesLeft(iteratorId);

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = gdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ss += String.Format("Item with gid: 0x{0:x16}\n", rds[i].Properties.Find(r => r.Id == ModelCode.IDOBJ_GID).AsLong());
                        foreach (Property p in rds[i].Properties)
                        {
                            if (p.Id == ModelCode.IDOBJ_GID && gidBool == false)
                            {

                            }
                            else
                            {
                                ss += String.Format("\t{0} =", p.Id);
                                switch (p.Type)
                                {
                                    case PropertyType.Float:
                                        ss += String.Format(" {0}:\n", p.AsFloat());
                                        break;
                                    case PropertyType.Enum:
                                        try
                                        {
                                            EnumDescs enumDescs = new EnumDescs();
                                            ss += String.Format(" {0}\n", enumDescs.GetStringFromEnum(p.Id, p.AsEnum()));
                                        }
                                        catch
                                        {
                                            ss += String.Format(" {0}\n", p.AsEnum().ToString());
                                        }
                                        break;
                                    case PropertyType.Bool:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsBool());
                                        }
                                        break;
                                    case PropertyType.Int32:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsInt());
                                        }
                                        break;
                                    case PropertyType.Int64:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsLong());
                                        }
                                        break;
                                    case PropertyType.DateTime:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsDateTime());
                                        }
                                        break;
                                    case PropertyType.Reference:
                                        ss += (String.Format(" 0x{0:x16}\n", p.AsReference()));
                                        break;
                                    case PropertyType.String:
                                        if (p.PropertyValue.StringValue == null)
                                        {
                                            p.PropertyValue.StringValue = String.Empty;
                                        }
                                        ss += String.Format(" {0}\n", p.AsString());
                                        break;
                                    case PropertyType.ReferenceVector:
                                        if (p.AsLongs().Count > 0)
                                        {
                                            string s = "";
                                            for (int j = 0; j < p.AsLongs().Count; j++)
                                            {
                                                s += (String.Format(" 0x{0:x16},\n", p.AsLongs()[j]));
                                            }
                                            ss += s;//(sb.ToString(0, sb.Length - 2));
                                        }
                                        else
                                        {
                                            ss += ("empty long/reference vector\n");
                                        }
                                        break;
                                    default:
                                        throw new Exception("Failed to export Resource Description as XML. Invalid property type.");

                                }
                            }
                        }
                    }
                    resourcesLeft = gdaQueryProxy.IteratorResourcesLeft(iteratorId);
                }

                gdaQueryProxy.IteratorClose(iteratorId);


            }
            catch (Exception)
            {

            }

            return ss;
        }
        public ModelCode GetModelCodeFromId(long id)
        {
            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(id);

            ModelCode modelCode;
            if (this.modelResourcesDesc.type2modelCode.TryGetValue(type, out modelCode))
            {
                return modelCode;
            }
            else
            {
                string message = string.Format("Failed to get model type code for DMS type: {0}. Invalid DMS type. ID = 0x{1:x16}", type, id);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception(message);
            }
        }
        public List<ModelCode> NadjiAtributeMC(ModelCode modelKod)
        {
            ModelResourcesDesc modResDes = new ModelResourcesDesc();
            List<ModelCode> lista = modResDes.GetAllPropertyIds(modelKod);

            return lista;
        }

        public List<ModelCode> NadjiAtrIDs(long gid3)
        {
            ModelResourcesDesc modResDes = new ModelResourcesDesc();
            List<ModelCode> lista = modResDes.GetAllPropertyIdsForEntityId(gid3);
            List<ModelCode> rez = new List<ModelCode>();

            foreach (ModelCode mc in lista)
            {
                if (Property.GetPropertyType(mc) == PropertyType.Reference || Property.GetPropertyType(mc) == PropertyType.ReferenceVector)
                {
                    rez.Add(mc);
                }

            }

            return rez;

        }

        public string GetExtentValues(DMSType type, List<ModelCode> props)
        {

            int iteratorId = 0;
            List<long> ids = new List<long>();
            string ss = "";
            bool gidBool = true;
            ModelCode modelCode = modelResourcesDesc.GetModelCodeFromType(type);
            try
            {
                int numberOfResources = 2;
                int resourcesLeft = 0;

                List<ModelCode> properties = props;// modelResourcesDesc.GetAllPropertyIds(modelCode);
                if (props.Contains(ModelCode.IDOBJ_GID) == false)
                {
                    properties.Add(ModelCode.IDOBJ_GID);
                    gidBool = false;
                }
                iteratorId = gdaQueryProxy.GetExtentValues(modelCode, properties);
                resourcesLeft = gdaQueryProxy.IteratorResourcesLeft(iteratorId);
                ss += String.Format("Items with ModelCode: {0}:\n", modelCode.ToString());
                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = gdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ss += String.Format("\tItem with gid: 0x{0:x16}\n", rds[i].Properties.Find(r => r.Id == ModelCode.IDOBJ_GID).AsLong());
                        foreach (Property p in rds[i].Properties)
                        {
                            if (p.Id == ModelCode.IDOBJ_GID && gidBool == false)
                            {

                            }
                            else
                            {
                                ss += String.Format("\t\t{0} =", p.Id);
                                switch (p.Type)
                                {
                                    case PropertyType.Float:
                                        ss += String.Format(" {0}:\n", p.AsFloat());
                                        break;
                                    case PropertyType.Enum:
                                        try
                                        {
                                            EnumDescs enumDescs = new EnumDescs();
                                            ss += String.Format(" {0}\n", enumDescs.GetStringFromEnum(p.Id, p.AsEnum()));
                                        }
                                        catch
                                        {
                                            ss += String.Format(" {0}\n", p.AsEnum().ToString());
                                        }
                                        break;
                                    case PropertyType.Bool:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsBool());
                                        }
                                        break;
                                    case PropertyType.Int32:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsInt());
                                        }
                                        break;
                                    case PropertyType.Int64:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsLong());
                                        }
                                        break;
                                    case PropertyType.DateTime:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ss += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ss += String.Format(" {0}\n", p.AsDateTime());
                                        }
                                        break;
                                    case PropertyType.Reference:
                                        ss += (String.Format(" 0x{0:x16}\n", p.AsReference()));
                                        break;
                                    case PropertyType.String:
                                        if (p.PropertyValue.StringValue == null)
                                        {
                                            p.PropertyValue.StringValue = String.Empty;
                                        }
                                        ss += String.Format(" {0}\n", p.AsString());
                                        break;
                                    case PropertyType.ReferenceVector:
                                        if (p.AsLongs().Count > 0)
                                        {
                                            string s = "";
                                            for (int j = 0; j < p.AsLongs().Count; j++)
                                            {
                                                s += (String.Format("0x{0:x16},\n", p.AsLongs()[j]));
                                            }
                                            ss += s;
                                        }
                                        else
                                        {
                                            ss += ("empty long/reference vector\n");
                                        }
                                        break;
                                    default:
                                        throw new Exception("Failed to export Resource Description as XML. Invalid property type.");
                                }

                            }
                        }
                    }
                    resourcesLeft = gdaQueryProxy.IteratorResourcesLeft(iteratorId);
                }

                gdaQueryProxy.IteratorClose(iteratorId);

            }
            catch (Exception)
            {

            }


            return ss;
        }

        public DMSType ConvertModelCodeToDMSType(ModelCode model)
        {
            switch (model)
            {
                case ModelCode.TERMINAL:
                    return DMSType.TERMINAL;
                case ModelCode.EQUIPMENTCONTAINER:
                    return DMSType.EQUIPMENTCONTAINER;
                case ModelCode.CONNECTIVITYNODE:
                    return DMSType.CONNECTIVITYNODE;
                case ModelCode.SWITCH:
                    return DMSType.SWITCH;
                case ModelCode.MEASUREMENT:
                    return DMSType.MEASUREMENT;
                default:
                    throw new ArgumentException($"Unsupported ModelCode: {model}");
            }
        }
        public ModelCode ConvertDMSTypeToModelCode(DMSType dmsType)
        {
            switch (dmsType)
            {
                case DMSType.TERMINAL:
                    return ModelCode.TERMINAL;
                case DMSType.SWITCH:
                    return ModelCode.SWITCH;
                case DMSType.MEASUREMENT:
                    return ModelCode.MEASUREMENT;
                case DMSType.EQUIPMENTCONTAINER:
                    return ModelCode.EQUIPMENTCONTAINER;
                case DMSType.CONNECTIVITYNODE:
                    return ModelCode.CONNECTIVITYNODE;
                default:
                    throw new ArgumentException($"Unsupported DMSType: {dmsType}");
            }
        }
        public List<ModelCode> GetDMSTypes()
        {
            List<ModelCode> retval = new List<ModelCode>();

            foreach (DMSType dmsType in Enum.GetValues(typeof(DMSType)))
            {
                if (dmsType != DMSType.MASK_TYPE)
                {
                    ModelCode modelCode = ConvertDMSTypeToModelCode(dmsType);
                    retval.Add(modelCode);
                }
            }

            return retval;
        }


        #endregion GDAQueryService

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}