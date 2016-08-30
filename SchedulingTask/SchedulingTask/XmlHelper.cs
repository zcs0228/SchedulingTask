using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XmlAdapter.XmlRepository
{
    public class XmlHelper
    {
        private string _filePath;

        public XmlHelper(string filePath, string rootName = "Root")
        {
            _filePath = filePath;
            if (!File.Exists(filePath))
            {
                CreateDocument(rootName);
            }
        }

        /// <summary>
        /// 创建XML文件
        /// </summary>
        public void CreateDocument(string rootName)
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(rootName));
            xdoc.Save(_filePath);
        }
        public void ReadXML(string xml)
        {
            XElement xEle = XElement.Parse(xml);
        }
        /// <summary>
        /// 向XML中添加元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="childXElements">包含的子元素</param>
        public void AddXElement(string xName, params XElement[] childXElements)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement newNode = new XElement(xName, childXElements);
            rootNode.Add(newNode);
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 向XML中添加元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="xValue">元素值</param>
        public void AddXElement(string xName, object xValue)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement newNode = new XElement(xName, xValue);
            rootNode.Add(newNode);
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 从XML中删除元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        public void DeleteXElement(string xName)
        {
            XElement rootNode = XElement.Load(_filePath);

            IEnumerable<XElement> targetNodes = from target in rootNode.Descendants(xName) 
                                                select target;
            targetNodes.Remove();
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 更新元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="xValue">元素值</param>
        public void UpdateXElement(string xName, object xValue)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                                select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            targetNode.SetValue(xValue);

            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 增加或修改元素值
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="xValue">元素值</param>
        public void AddOrUpdateXElement(string xName, object xValue)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                AddXElement(xName, xValue);
            }
            else
            {
                targetNode.SetValue(xValue);
            }
        }
        /// <summary>
        /// 查询元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <returns></returns>
        public IEnumerable<XElement> QueryXElement(string xName)
        {
            XElement rootNode = XElement.Load(_filePath);
            IEnumerable<XElement> targetNodes = from target in rootNode.Descendants(xName)
                                                select target;
            return targetNodes;
        }
        public object QueryXElementValue(XElement sourceXElement, string xName)
        {
            IEnumerable<XElement> targetNodes = from target in sourceXElement.Descendants(xName)
                                                select target;
            if (targetNodes == null || targetNodes.Count() == 0)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            return targetNodes.FirstOrDefault().Value;
        }
        /// <summary>
        /// 查询元素值
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <returns></returns>
        public object QueryXElementValue(string xName)
        {
            XElement rootNode = XElement.Load(_filePath);
            IEnumerable<XElement> targetNodes = from target in rootNode.Descendants(xName)
                                                select target;
            if (targetNodes == null || targetNodes.FirstOrDefault() == null)
            {
                //throw new Exception("不存在名称为【" + xName + "】的元素！");
                return null;
            }
            else
            {
                return targetNodes.FirstOrDefault().Value;
            }
        }

        /// <summary>
        /// 元素添加属性
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="xAttribute">属性对象</param>
        public void AddOrUpdateXAttributeForXElement(string xName, XAttribute[] xAttribute)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            foreach(var item in xAttribute)
            {
                XAttribute attribute = targetNode.Attribute(item.Name);
                if (attribute != null)
                {
                    attribute.SetValue(item.Value);
                }
                else
                {
                    targetNode.Add(item);
                }
            }
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 在指定的元素后面添加元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="xElements">需要添加的元素</param>
        public void AddElementToAfter(string xName, XElement[] xElements)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            targetNode.AddAfterSelf(xElements);
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 在指定的元素前面添加元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="xElements">需要添加的元素</param>
        public void AddElementToBefore(string xName, XElement[] xElements)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            targetNode.AddBeforeSelf(xElements);
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 元素添加属性
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeValue">属性值</param>
        public void AddAttributeToXElement(string xName, string attributeName, object attributeValue)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            targetNode.SetAttributeValue(attributeName, attributeValue);
            rootNode.Save(_filePath);
        }
        public void UpdateAttributeToXElement(string xName, string attributeName, object attributeValue)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            targetNode.SetAttributeValue(attributeName, attributeValue);
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 获取元素指定属性值
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns></returns>
        public string QueryAttributeFromXElement(string xName, string attributeName)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            return targetNode.Attribute(attributeName).Value.ToString();
        }
        /// <summary>
        /// 在指定元素后添加注释
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="commentText">注释</param>
        public void AddCommentToAfter(string xName, string commentText)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            XComment xcom = new XComment(commentText);
            targetNode.AddAfterSelf(xcom);
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 在指定元素前添加注释
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="commentText">注释</param>
        public void AddCommentToBefore(string xName, string commentText)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            XComment xcom = new XComment(commentText);
            targetNode.AddBeforeSelf(xcom);
            rootNode.Save(_filePath);
        }
        /// <summary>
        /// 替换指定的元素
        /// </summary>
        /// <param name="xName">元素名称</param>
        /// <param name="xValue">元素值</param>
        /// <param name="newElement">新元素</param>
        public void ReplaceXElement(string xName, string xValue, XElement newElement)
        {
            XElement rootNode = XElement.Load(_filePath);
            XElement targetNode = (from target in rootNode.Descendants(xName)
                                   where target.Value.Equals(xValue)
                                   select target).FirstOrDefault();
            if (targetNode == null)
            {
                throw new Exception("不存在名称为【" + xName + "】的元素！");
            }
            targetNode.ReplaceWith(newElement);
            rootNode.Save(_filePath);
        }

        //XElement.Descendants() 获取元素的所有子元素
        //XElement.DescendantsAndSelf() 获取元素的所有子元素（包含自己）
        //XElement.Ancestors() 获取元素的所有父元素
        //XElement.AncestorsAndSelf() 获取元素的所有父元素（包含自己）
        //XElement.ElementsBeforeSelf() 获取同级元素之前的元素
        //XElement.ElementsAfterSelf() 获取同级元素之后的元素
    }
}
