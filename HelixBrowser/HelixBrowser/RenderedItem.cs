using System;
using System.Collections.Generic;
using System.Text;
using HelixBrowser.Properties;
using VX.Knowledge.DataSource;
using VX.Knowledge.DataSource.Model;
using VX.Storage;
using VX.Sys;

namespace HelixBrowser
{
	public class RenderedItem
	{
		public int Indent;
		public bool Up;
		public VXInfoItem InfoItem;

		public string Render()
		{
			string html = Resources.RenderItem;
			string indentHtml = RenderIndent();
			string propertiesHtml = RenderProperties();

			string contentHtml = String.Empty;
			string codeHtml = String.Empty;

			if (InfoItem.Template.ID == KB.Template.Infonode.Id)
			{
				contentHtml = RenderInfoNode();
			}
			else if (InfoItem.Template.ID == KB.Template.Group.Id)
			{
				codeHtml = "Group";
				contentHtml = RenderGroup();
			}
			else if (InfoItem.Template.ID == KB.Template.Mkt.Id)
			{
				codeHtml = "Mkt";
				contentHtml = RenderMarketingDescription();
			}
			else if (InfoItem.Template.ID == KB.Template.Ksp.Id)
			{
				codeHtml = "Ksp";
				contentHtml = RenderKeySellingPoints();
			}
			else if (InfoItem.Template.ID == KB.Template.Images.Id)
			{
				codeHtml = "Image";
				contentHtml = RenderImages();
			}
			else if (InfoItem.Template.ID == KB.Template.Multipleimages.Id)
			{
				codeHtml = "Image";
				contentHtml = RenderMultipleImages();
			}
			else if (KB.Parameter.BrowserIsContentAttribute.Read(InfoItem.Template))
			{
				codeHtml = "Attribute";
				contentHtml = RenderAttributes();
			}

			html = html.Replace("{CODE}", codeHtml);
			html = html.Replace("{INDENT}", indentHtml);
			html = html.Replace("{CONTENT}", contentHtml);
			html = html.Replace("{PROPERTIES}", propertiesHtml);

			return html;
		}

		private string RenderIndent()
		{
			return String.Format("style=\"margin-left: {0}px;\"", 60 * Indent);
		}

		private string RenderProperties()
		{
			string template = InfoItem.Template.Name.GetBestString();
			string id = InfoItem.ID.ToString();
			string created = InfoItem.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
			string updated = InfoItem.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss");
			string owner = InfoItem.OwnerID.ToString();

			string html = Resources.RenderProperties;
			html = html.Replace("{TEMPLATE}", template);
			html = html.Replace("{ID}", id);
			html = html.Replace("{CREATED}", created);
			html = html.Replace("{UPDATED}", updated);
			html = html.Replace("{OWNER}", owner);

			return html;
		}

		private string RenderInfoNode()
		{
			InfoNode node = InfoItem;

			string iconHtml = Up ? "folder_up.png" : "folder.png";
			string typeHtml = node.TypeValue;
			string titleHtml = node.Title.GetBestString();

			string idHtml = Resources.Empty;
			switch (node.Type)
			{
				case InfoNodeType.Brand:
				case InfoNodeType.ProductLine:
					idHtml = String.Format(
						Resources.ProductLineId,
						node.ProductLineId.HasValue ? node.ProductLineId.ToString() : Resources.Empty);
					break;

				case InfoNodeType.Model:
					idHtml = String.Format(
						Resources.ModelId,
						node.ModelId.HasValue ? node.ModelId.ToString() : Resources.Empty);
					break;

				case InfoNodeType.SubModel:
					idHtml = String.Format(
						Resources.DsAttribute,
						!String.IsNullOrEmpty(node.DsAttribute) ? node.DsAttribute : Resources.Empty);
					break;

				case InfoNodeType.Pn:
					idHtml = String.Format(
						Resources.SkuId,
						node.SkuId.HasValue ? node.SkuId.ToString() : Resources.Empty);
					break;
			}

			string html = Resources.RenderInfoNode;
			html = html.Replace("{ICON}", iconHtml);
			html = html.Replace("{TYPE}", typeHtml);
			html = html.Replace("{TITLE}", titleHtml);
			html = html.Replace("{ID}", idHtml);

			return html;
		}

		private string RenderGroup()
		{
			Group group = InfoItem;

			string typeHtml = group.TypeValue;

			List<string> parts = new List<string>();
			foreach (RuleItem rule in group.GetAllRules())
			{
				List<string> values = new List<string>();
				foreach (VXString value in rule.AttributeValue.ReadAll())
				{
					values.Add(value);
				}

				string op = Resources.Unknown;
				if (rule.Operation == RuleOperation.Equals)
					op = "=";
				else if (rule.Operation == RuleOperation.NotEquals)
					op = "!=";

				string part = String.Format(
					"{0} {1} {2}",
					rule.AttributeName,
					op,
					String.Join(", ", values.ToArray()));

				parts.Add(part);
			}

			string rulesHtml = String.Join("; ", parts.ToArray());

			string html = Resources.RenderGroup;
			html = html.Replace("{TYPE}", typeHtml);
			html = html.Replace("{RULES}", rulesHtml);

			return html;
		}

		private string RenderMarketingDescription()
		{
			StringBuilder sb = new StringBuilder();
			RenderText(sb, KB.Attribute.Marketingtext.Qn, Resources.RenderMarketingDescription);
			return sb.ToString();
		}

		private string RenderKeySellingPoints()
		{
			StringBuilder sb = new StringBuilder();
			RenderText(sb, KB.Attribute.Ksptext.Qn, Resources.RenderKeySellingPoints);
			RenderText(sb, KB.Attribute.KsptextLong.Qn, Resources.RenderProductFeatures);
			return sb.ToString();
		}

		private string RenderImages()
		{
			StringBuilder sb = new StringBuilder();

			foreach (VXValueGroup vg in InfoItem.GetAllValueGroups(KB.Attribute.Image.Qn))
			{
				string typeHtml = Resources.Unknown;
				string typeQn = vg.ReadEnumValueQN(KB.Attribute.Imagetype.Qn);
				if (typeQn == KB.Attribute.Imagetype.StandardQn)
					typeHtml = "STD";
				else if (typeQn == KB.Attribute.Imagetype.ReworkedQn)
					typeHtml = "REWORK";
				else if (typeQn == KB.Attribute.Imagetype.RawQn)
					typeHtml = "RAW";
				else if (typeQn == KB.Attribute.Imagetype.MediumQn)
					typeHtml = "MEDIUM";
				else if (typeQn == KB.Attribute.Imagetype.LargeQn)
					typeHtml = "LARGE";
				else if (typeQn == KB.Attribute.Imagetype.CnetmediumQn)
					typeHtml = "CNET";
				else if (typeQn == KB.Attribute.Imagetype.HighresolutionQn)
					typeHtml = "HI-RES";

				VXFile file = vg.ReadValue<VXFile>(KB.Attribute.Image.Qn);
				string urlHtml = GetFileUrl(KB.Template.Images.Id, file);

				string html = Resources.RenderImages;
				html = html.Replace("{TYPE}", typeHtml);
				html = html.Replace("{URL}", urlHtml);

				sb.AppendLine(html);
			}

			return sb.ToString();
		}

		private string RenderMultipleImages()
		{
			StringBuilder sb = new StringBuilder();

			foreach (VXValueGroup vg in InfoItem.GetAllValueGroups(KB.Attribute.Image.Qn))
			{
				string typeHtml = Resources.Unknown;
				string typeQn = vg.ReadEnumValueQN(KB.Attribute.MultipleimagesContext.Qn);
				if (typeQn == null)
					typeHtml = Resources.Empty;
				else if (typeQn == KB.Attribute.MultipleimagesContext.ComponentQn)
					typeHtml = "COM";
				else if (typeQn == KB.Attribute.MultipleimagesContext.LicenseQn)
					typeHtml = "LIC";
				else if (typeQn == KB.Attribute.MultipleimagesContext.LifestyleQn)
					typeHtml = "LIF";
				else if (typeQn == KB.Attribute.MultipleimagesContext.LogotypeQn)
					typeHtml = "LOG";
				else if (typeQn == KB.Attribute.MultipleimagesContext.PackageviewQn)
					typeHtml = "PAC";
				else if (typeQn == KB.Attribute.MultipleimagesContext.ProductshotQn)
					typeHtml = "PRO";
				else if (typeQn == KB.Attribute.MultipleimagesContext.SchematicQn)
					typeHtml = "SCH";
				else if (typeQn == KB.Attribute.MultipleimagesContext.ScreenshotQn)
					typeHtml = "SCR";
				else if (typeQn == KB.Attribute.MultipleimagesContext.WhatsintheboxQn)
					typeHtml = "WHA";

				string angleHtml = Resources.Unknown;
				string angleQn = vg.ReadEnumValueQN(KB.Attribute.MultipleimagesAngle.Qn);
				if (angleQn == null)
					angleHtml = Resources.Empty;
				else if (angleQn == KB.Attribute.MultipleimagesAngle.BackQn)
					angleHtml = "BAC";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.BottomQn)
					angleHtml = "BOT";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.CloseupQn)
					angleHtml = "CLO";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.FrontQn)
					angleHtml = "FRO";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.InsideQn)
					angleHtml = "INS";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.LeftangleQn)
					angleHtml = "L-A";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.LeftsideQn)
					angleHtml = "L-S";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.MultiangleQn)
					angleHtml = "M-A";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.PortsQn)
					angleHtml = "POR";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.RightangleQn)
					angleHtml = "R-A";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.RightsideQn)
					angleHtml = "R-S";
				else if (angleQn == KB.Attribute.MultipleimagesAngle.TopQn)
					angleHtml = "TOP";

				VXFile file = vg.ReadValue<VXFile>(KB.Attribute.Image.Qn);
				string urlHtml = GetFileUrl(KB.Template.Multipleimages.Id, file);

				string html = Resources.RenderMultipleImages;
				html = html.Replace("{TYPE}", typeHtml);
				html = html.Replace("{ANGLE}", angleHtml);
				html = html.Replace("{URL}", urlHtml);

				sb.AppendLine(html);
			}

			return sb.ToString();
		}

		private string RenderAttributes()
		{
			List<string> parts = new List<string>();
			foreach (VXAttributeGroup group in InfoItem.Template.Groups)
			{
				if (group.QualifiedName.FullName.EndsWith(".groups.main"))
					continue;

				VXValueGroupList vgl = InfoItem[group];
				foreach (VXValueGroup vg in vgl)
				{
					List<string> attrParts = new List<string>();
					foreach (VXFeaturedAttribute attr in group)
					{
						List<string> valueParts = new List<string>();
						foreach (IVXValueType value in vg.ReadUntypedValues(attr.QualifiedName))
						{
							string valuePart = value.ToString();
							valueParts.Add(valuePart);
						}

						if (valueParts.Count > 0)
						{
							string attrPart = String.Join(", ", valueParts.ToArray());
							attrParts.Add(attrPart);
						}
					}

					if (attrParts.Count > 0)
					{
						string part = String.Join(" - ", attrParts.ToArray());
						parts.Add(part);
					}
				}
			}

			string attrsHtml = String.Join("; ", parts.ToArray());

			string html = Resources.RenderAttributes;
			html = html.Replace("{ATTRS}", attrsHtml);

			return html;
		}

		private void RenderText(StringBuilder sb, VXQualifiedName attrQn, string renderFormat)
		{
			VXMString text = InfoItem.ReadFirstValue<VXMString>(attrQn);
			foreach (VXLanguageType lang in text)
			{
				string langHtml = VXLanguage.ConvertToCode(lang);
				string textHtml = StringHelper.ToSingleLine(text[lang]);

				string html = renderFormat;
				html = html.Replace("{LANG}", langHtml);
				html = html.Replace("{TEXT}", textHtml);

				sb.AppendLine(html);
			}
		}

		private static string GetFileUrl(VXGuid templateId, VXFile file)
		{
			string fileId = file.FileID.ToString();
			return String.Format(
				"{0}/{1}/{2}/{3}/{4}{5}",
				"http://rufrt-dev04/VXShare",
				//"http://rufrt-test11/VXShare_real",
				//"http://vx-ws.cnetcontentsolutions.com/vxshare",
				templateId,
				fileId.Substring(0, 2),
				fileId.Substring(2, 2),
				fileId,
				file.FileExtension);
		}
	}
}