using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using DFAIT.Interwoven.Web.Component;
using DFAIT.Interwoven.Web.Service;
using DFAIT.Interwoven.Web.Shared;
using DFAIT.Interwoven.Web.Utilities;

namespace DFAIT.Interwoven.Web.Component.Forms
{
    public class Widget : IWidget
    {
        private JToken _JToken;

        private Hashtable _ParameterHash;
        private string _HTML = "";
        private string[] _formElements;
        private string _LanguageCode;
        private string _FullLanguageName = "";
        private string _TextDirection = "";
        private bool _FieldValidationExists;
        private Enums.ViewType _ViewType = Enums.ViewType.Target;

        private string _Parameters = null;

        public Widget(JToken JToken)
        {
            _JToken = JToken;

            Enums.ViewType _ViewType = DFAIT.Interwoven.Web.Utilities.Utility.GetViewType();

            this._Parameters = this._JToken.SelectToken("parameters").ToString();
            this._ParameterHash = GetData();
            this._LanguageCode = this._JToken.SelectToken("language_code").ToString();
            this._FullLanguageName = Languages.GetLanguageAttribute("short", this._LanguageCode, "name", this._ViewType);
            this._TextDirection = Languages.GetLanguageAttribute("short", this._LanguageCode, "dir", this._ViewType);            
        }

        /// <summary>
        /// convert TinyMCE data to a Hashtable for processing
        /// </summary>
        /// <returns></returns>
        private Hashtable GetData()
        {
            Hashtable _Hashtable = new Hashtable();
            string pattern = @"(.*?)\|=\|(.*?)(\|&\||-->|$)";
            foreach (Match match in Regex.Matches(this._Parameters, pattern))
            {
                _Hashtable.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
            return _Hashtable;
        }

        public string Render()
        {
            try
            {
                StringBuilder _StringBuilder = new StringBuilder();

                string param = "";
                string formClass = "onlineform";
                string formAction = "/iwglobal/utilities/formEmailSubmit.aspx";
                string formId = "onlineForm";

                string _LanguageEnglish = Languages.GetLanguageAttribute("short", Constants.ENGLISH, "name", this._ViewType);
                string _LangAttribute = null;

                param = _ParameterHash["formparameter"].ToString();   
                param = HttpUtility.UrlDecode(param);
                param = param.Substring(param.IndexOf("[FormParametersBegin]") + 21, param.LastIndexOf("formElementseperator,") - param.IndexOf("[FormParametersBegin]") - 21);

                _formElements = Regex.Split(param, "formElementseperator,");

                string formTitle = getFieldsetElementValue(0, 0, "form_title");
                string form_dcp = getFieldsetElementValue(0, 0, "form_dcp");
                string form_submitbuttontext = getFieldsetElementValue(0, 0, "form_submitbuttontext");
                string form_resetbuttontext = getFieldsetElementValue(0, 0, "form_resetbuttontext");
                string form_buttonalignment = getFieldsetElementValue(0, 0, "form_buttonalignment");
                string form_sendto = Security.EncryptString(getFieldsetElementValue(0, 0, "form_sendto"), Constants.ENCRYPTIONDECRYPTIONKEY);
                string form_sendfrom = Security.EncryptString(getFieldsetElementValue(0, 0, "form_sendfrom"), Constants.ENCRYPTIONDECRYPTIONKEY);
                string form_cc = Security.EncryptString(getFieldsetElementValue(0, 0, "form_cc"), Constants.ENCRYPTIONDECRYPTIONKEY);
                string form_bcc = Security.EncryptString(getFieldsetElementValue(0, 0, "form_bcc"), Constants.ENCRYPTIONDECRYPTIONKEY);
                string form_subject = getFieldsetElementValue(0, 0, "form_subject");
                string form_redirect = getFieldsetElementValue(0, 0, "form_redirect");
                string form_submitformat = getFieldsetElementValue(0, 0, "form_submitformat");

                // set up the referer for the 'Cancel' button
                string _Referer = null;
                if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
                {
                    _Referer = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
                }

                _HTML += "<form runat=\"server\" class=\"" + formClass + "\" dir=\"" + _TextDirection + "\" method=\"post\" id=\"" + formId + "\">";
                _HTML += "<div>";
                _HTML += "<input type=\"hidden\" name=\"form_sendto\" value=\"" + form_sendto + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_sendfrom\" value=\"" + form_sendfrom + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_dcp\" value=\"" + form_dcp + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_cc\" value=\"" + form_cc + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_bcc\" value=\"" + form_bcc + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_subject\" value=\"" + form_subject + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_redirect\" value=\"" + form_redirect + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_submitformat\" value=\"" + form_submitformat + "\" />";
                _HTML += "<input type=\"hidden\" name=\"form_Referer\" value=\"" + _Referer + "\" />";
                _HTML += "</div>";

                int fieldsetId;
                int fieldsetElementId;
                bool[] fieldsetVisited = new bool[getFieldsetCount()];

                for (int i = 0; i < _formElements.Length; i++)
                {
                    fieldsetId = int.Parse(getFieldset(_formElements[i]));
                    fieldsetElementId = int.Parse(getFieldsetElement(_formElements[i]));

                    if ((fieldsetId > 0) && (fieldsetVisited[fieldsetId - 1] != true))
                    {
                        buildFieldsetHTML(fieldsetId);
                        fieldsetVisited[fieldsetId - 1] = true;
                    }
                }

                _HTML += "<div class=\"formbuttonalign" + form_buttonalignment + "\">";

                _LangAttribute = null; //reset this back 
                string _SubmitLabel = Languages.GetLanguageString("/config/display/submit/language", _FullLanguageName, "content", this._ViewType);
                if (String.IsNullOrEmpty(_SubmitLabel)) // get English label if alternate language is NOT available AND add lang attribute to button
                {
                    _SubmitLabel = Languages.GetLanguageString("/config/display/submit/language", _LanguageEnglish, "content", this._ViewType);
                    _LangAttribute = " lang=\"" + Constants.ENGLISH_2LETTER + "\" xml:lang=\"" + Constants.ENGLISH_2LETTER + "\" ";
                }

                //Submit button
                _HTML += "<asp:Button CssClass=\"formbutton\" " + _LangAttribute + " text=\"" + form_submitbuttontext + "\" postbackurl=\"" + formAction + "\" title=\"" + _SubmitLabel + "\" runat=\"server\" id=\"Submit\" />";

                _LangAttribute = null; //reset this back
                string _ResetLabel = Languages.GetLanguageString("/config/display/reset/language", _FullLanguageName, "content", this._ViewType);
                if (String.IsNullOrEmpty(_ResetLabel)) // get English label if alternate language is NOT available AND add lang attribute to button
                {
                    _ResetLabel = Languages.GetLanguageString("/config/display/reset/language", _LanguageEnglish, "content", this._ViewType);
                    _LangAttribute = " lang=\"" + Constants.ENGLISH_2LETTER + "\" xml:lang=\"" + Constants.ENGLISH_2LETTER + "\" ";
                }
                //Reset button
                _HTML += "<input class=\"formbutton\" type=\"reset\" value=\"" + _ResetLabel + "\" title=\"" + _ResetLabel + "\" " + _LangAttribute + " />";

                //Cancel button
                if (System.Web.HttpContext.Current.Request.UrlReferrer != null) // only display button if the form was linked to from elsewhere, and NOT from a bookmark
                {
                    string _CancelLabel = Languages.GetLanguageString("/config/display/cancel/language", _LanguageEnglish, "content", this._ViewType);
                    _HTML += "<asp:Button CssClass=\"formbutton\" id=\"button_cancel\" runat=\"server\" Text=\"" + _CancelLabel + "\" CausesValidation=\"False\" postbackurl=\"" + formAction + "\"" + "></asp:Button>";
                }

                _HTML += "</div>"; // close formbuttonalign
                _HTML += "</form>";

                _StringBuilder.Append(_HTML);
                return _StringBuilder.ToString();
            }
            catch (Exception ex)
            {
                // error logging mechanism to be implemented in the future
                return "";
            }
        }

        private string getMandatoryLabel()
        {
            string _LanguageEnglish = Languages.GetLanguageAttribute("short", Constants.ENGLISH, "name", this._ViewType);
            string _LangAttribute = null; //reset this back
            string _Label = Languages.GetLanguageString("/config/display/mandatory/language", this._FullLanguageName, "content", this._ViewType);
            if (String.IsNullOrEmpty(_Label)) // get English label is alternate language is NOT available AND add lang attribute to button
            {
                _Label = Languages.GetLanguageString("/config/display/mandatory/language", _LanguageEnglish, "content", this._ViewType);
                _LangAttribute = " lang=\"" + Constants.ENGLISH_2LETTER + "\" xml:lang=\"" + Constants.ENGLISH_2LETTER + "\" ";
            }

            if (!String.IsNullOrEmpty(_Label))
            {
                if (!String.IsNullOrEmpty(_LangAttribute)) // alternate language so add 'lang' attribute
                {
                    return "<span " + _LangAttribute + ">" + _Label.Replace("*", "<span class=\"warning\"> *</span>") + "</span>"; // add some styling to the *
                }
                return _Label.Replace("*", "<span class=\"warning\"> *</span>"); // add some styling to the *
            }
            return _Label;
        }

        private string printFormElements()
        {
            string _printFormElements = "";
            for (int i = 0; i < _formElements.Length; i++)
            {
                _printFormElements += _formElements[i] + "<br />";
            }
            return _printFormElements;
        }

        private int getFieldsetCount()
        {
            int fieldsetCount = 0;
            for (int i = 0; i < _formElements.Length; i++)
            {
                if (int.Parse(getFieldset(_formElements[i])) > fieldsetCount)
                {
                    fieldsetCount = int.Parse(getFieldset(_formElements[i]));
                }
            }
            return fieldsetCount;
        }

        private int getFieldsetElementCount(int fieldsetId)
        {
            int fieldsetElementCount = 0;
            for (int i = 0; i < _formElements.Length; i++)
            {
                if (int.Parse(getFieldset(_formElements[i])) == fieldsetId)
                {
                    if (int.Parse(getFieldsetElement(_formElements[i])) > fieldsetElementCount)
                    {
                        fieldsetElementCount = int.Parse(getFieldsetElement(_formElements[i]));
                    }
                }
            }
            return fieldsetElementCount;
        }

        private void buildFieldsetHTML(int fieldsetId)
        {
            string fieldset_visible = getFieldsetElementValue(fieldsetId, 0, "fieldset_visible");
            string fieldset_layout = getFieldsetElementValue(fieldsetId, 0, "fieldset_layout");
            string fieldset_alignment = getFieldsetElementValue(fieldsetId, 0, "fieldset_alignment");
            string fieldset_width = getFieldsetElementValue(fieldsetId, 0, "fieldset_width");
            string fieldset_legend_caption = getFieldsetElementValue(fieldsetId, 0, "fieldset_legend_caption");

            _HTML += "<fieldset class=\"visible" + fieldset_visible + " " + fieldset_layout + " " + fieldset_alignment + fieldset_width + "\">";

            if (fieldset_legend_caption != "")
            {
                _HTML += "<legend>" + fieldset_legend_caption + "</legend>";
            }

            //*****************************************************************************************
            // check if any field was marked as 'required', and only then display the 
            int fieldsetElementId2 = 0;
            bool[] fieldsetElementVisited2 = new bool[getFieldsetElementCount(fieldsetId)];

            for (int i = 0; i < _formElements.Length; i++)
            {
                fieldsetElementId2 = int.Parse(getFieldsetElement(_formElements[i]));

                if ((fieldsetElementId2 > 0) && (int.Parse(getFieldset(_formElements[i])) == fieldsetId) && (fieldsetElementVisited2[fieldsetElementId2 - 1] != true))
                {
                    checkRequiredField(fieldsetId, fieldsetElementId2);
                }
            }
            checkRequiredField(fieldsetId, fieldsetElementId2);

            if (_FieldValidationExists)
            {
                _HTML += "<div class=\"formfieldrequired\">" + getMandatoryLabel() + "<br /></div>";
            }
            //*****************************************************************************************

            int fieldsetElementId;
            bool[] fieldsetElementVisited = new bool[getFieldsetElementCount(fieldsetId)];

            for (int i = 0; i < _formElements.Length; i++)
            {
                fieldsetElementId = int.Parse(getFieldsetElement(_formElements[i]));

                if ((fieldsetElementId > 0) && (int.Parse(getFieldset(_formElements[i])) == fieldsetId) && (fieldsetElementVisited[fieldsetElementId - 1] != true))
                {
                    buildFieldsetElementHTML(fieldsetId, fieldsetElementId);
                    fieldsetElementVisited[fieldsetElementId - 1] = true;
                }
            }
            _HTML += "</fieldset>";
        }

        private void buildFieldsetElementHTML(int fieldsetId, int fieldsetElementId)
        {
            switch (getFieldsetElementType(fieldsetId, fieldsetElementId))
            {
                case "text":
                    string text_label = getFieldsetElementValue(fieldsetId, fieldsetElementId, "text_label");
                    bool text_bold = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "text_bold") == "true") ? true : false;
                    bool text_italic = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "text_italic") == "true") ? true : false;
                    string text_maxlength = getFieldsetElementValue(fieldsetId, fieldsetElementId, "text_maxlength");
                    bool text_validation_required = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "text_validation_required") == "true") ? true : false;
                    string text_validation_required_error_message = Languages.GetLanguageString("config/display/FormValidationErrorMessages/RequiredField/language", this._FullLanguageName, "content", this._ViewType);
                    string text_validation_type = getFieldsetElementValue(fieldsetId, fieldsetElementId, "text_validation_type");
                    string text_validation_type_error_message = Languages.GetLanguageString("config/display/FormValidationErrorMessages/RegularExpression_" + text_validation_type + "/language", this._FullLanguageName, "content", this._ViewType);

                    _HTML += "<div><input type=\"hidden\" id=\"text_label_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"text_label_" + fieldsetId + "_" + fieldsetElementId + "\" value=\"" + text_label + "\" /></div>";

                    _HTML += "<div class=\"fieldsetElement\">";

                    _HTML += "<label for=\"text_" + fieldsetId + "_" + fieldsetElementId + "\">";
                    _HTML += getLabel(text_label, text_bold, text_italic, text_validation_required);
                    _HTML += "</label>";

                    _HTML += "<div class=\"group\"><asp:TextBox runat=\"server\" MaxLength=\"" + text_maxlength + "\" class=\"text\" id=\"text_" + fieldsetId + "_" + fieldsetElementId + "\" /><br />";
                    if (text_validation_required)
                    {
                        _FieldValidationExists = true;
                        _HTML += "<asp:RequiredFieldValidator runat=\"server\" ControlToValidate=\"text_" + fieldsetId + "_" + fieldsetElementId + "\" ErrorMessage=\" " + text_validation_required_error_message + "\" display=\"Dynamic\" />";
                    }

                    if (text_validation_type != "0")
                    {
                        _HTML += "<asp:RegularExpressionValidator runat=\"server\" ControlToValidate=\"text_" + fieldsetId + "_" + fieldsetElementId + "\" ValidationExpression=\"";

                        switch (text_validation_type)
                        {
                            case "email":
                                _HTML += "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";
                                break;
                            case "phone":
                                _HTML += "^[01]?[- .]?(\\([2-9]\\d{2}\\)|[2-9]\\d{2})[- .]?\\d{3}[- .]?\\d{4}$";
                                break;
                            case "numeric":
                                _HTML += "^\\d+$";
                                break;
                            case "postalcode":
                                _HTML += "^[ABCEGHJ-NPRSTVXY]{1}[0-9]{1}[ABCEGHJ-NPRSTV-Z]{1}[ ]?[0-9]{1}[ABCEGHJ-NPRSTV-Z]{1}[0-9]{1}$";
                                break;
                            case "zipcode":
                                _HTML += "^(\\d{5}-\\d{4}|\\d{5}|\\d{9})$|^([a-zA-Z]\\d[a-zA-Z] \\d[a-zA-Z]\\d)$";
                                break;
                        }

                        _HTML += "\" ErrorMessage=\" " + text_validation_type_error_message + "\" display=\"Dynamic\" />";
                    }

                    _HTML += "</div>";
                    _HTML += "<div class=\"floatfix\"></div>";
                    _HTML += "</div>";

                    break;
                case "password":
                    string password_label = getFieldsetElementValue(fieldsetId, fieldsetElementId, "password_label");
                    string password_maxlength = getFieldsetElementValue(fieldsetId, fieldsetElementId, "password_maxlength");
                    bool password_bold = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "password_bold") == "true") ? true : false;
                    bool password_italic = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "password_italic") == "true") ? true : false;
                    bool password_validation_required = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "password_validation_required") == "true") ? true : false;
                    string password_validation_confirm = getFieldsetElementValue(fieldsetId, fieldsetElementId, "password_validation_confirm"); // Not used
                    string password_validation_required_error_message = Languages.GetLanguageString("config/display/FormValidationErrorMessages/RequiredField/language", this._FullLanguageName, "content", this._ViewType);

                    _HTML += "<div><input type=\"hidden\" id=\"password_label_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"password_label_" + fieldsetId + "_" + fieldsetElementId + "\" value=\"" + password_label + "\" /></div>";

                    _HTML += "<div class=\"fieldsetElement\">";
                    
                    _HTML += "<label for=\"password_" + fieldsetId + "_" + fieldsetElementId + "\">";
                    _HTML += getLabel(password_label, password_bold, password_italic, password_validation_required);
                    _HTML += "</label>";

                    _HTML += "<div class=\"group\"><asp:TextBox runat=\"server\" TextMode=\"password\" MaxLength=\"" + password_maxlength + "\" class=\"text\" id=\"password_" + fieldsetId + "_" + fieldsetElementId + "\" /><br />";

                    if (password_validation_required)
                    {
                        _FieldValidationExists = true;
                        _HTML += "<asp:RequiredFieldValidator runat=\"server\" ControlToValidate=\"password_" + fieldsetId + "_" + fieldsetElementId + "\" ErrorMessage=\" " + password_validation_required_error_message + "\" display=\"Dynamic\" />";
                    }

                    _HTML += "</div>";
                    _HTML += "<div class=\"floatfix\"></div>";
                    _HTML += "</div>";

                    break;
                case "hidden":
                    string hidden_name = getFieldsetElementValue(fieldsetId, fieldsetElementId, "hidden_name");
                    string hidden_value = getFieldsetElementValue(fieldsetId, fieldsetElementId, "hidden_value");

                    _HTML += "<div><input type=\"hidden\" id=\"hidden_label_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"hidden_label_" + fieldsetId + "_" + fieldsetElementId + "\" value=\"" + hidden_name + "\" /></div>";
                    _HTML += "<div><input type=\"hidden\" id=\"hidden_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"" + hidden_name + "\" value=\"" + hidden_value + "\" /></div>";

                    break;
                case "textarea":
                    string textarea_label = getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_label");
                    string textarea_title = getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_title");
                    bool textarea_bold = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_bold") == "true") ? true : false;
                    bool textarea_italic = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_italic") == "true") ? true : false;
                    string textarea_numberofcolumns = getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_numberofcolumns");
                    string textarea_numberofrows = getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_numberofrows");
                    bool textarea_validation_required = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_validation_required") == "true") ? true : false;
                    string textarea_validation_required_error_message = Languages.GetLanguageString("config/display/FormValidationErrorMessages/RequiredField/language", this._FullLanguageName, "content", this._ViewType);

                    _HTML += "<div><input type=\"hidden\" id=\"textarea_label_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"textarea_label_" + fieldsetId + "_" + fieldsetElementId + "\" value=\"" + textarea_label + "\" /></div>";

                    _HTML += "<div class=\"fieldsetElement\">";

                    _HTML += "<label for=\"textarea_" + fieldsetId + "_" + fieldsetElementId + "\">";
                    _HTML += getLabel(textarea_label, textarea_bold, textarea_italic, textarea_validation_required);
                    _HTML += "</label>";

                    _HTML += "<div class=\"group\"><asp:TextBox runat=\"server\" id=\"textarea_" + fieldsetId + "_" + fieldsetElementId + "\" class=\"text\" TextMode=\"MultiLine\" Columns=\"" + textarea_numberofcolumns + "\" Rows=\"" + textarea_numberofrows + "\" /><br />";

                    if (textarea_validation_required)
                    {
                        _FieldValidationExists = true;
                        _HTML += "<asp:RequiredFieldValidator runat=\"server\" ControlToValidate=\"textarea_" + fieldsetId + "_" + fieldsetElementId + "\" ErrorMessage=\" " + textarea_validation_required_error_message + "\" display=\"Dynamic\" />";
                    }

                    _HTML += "</div>";
                    _HTML += "<div class=\"floatfix\"></div>";
                    _HTML += "</div>";

                    break;
                case "paragraph":
                    string paragraph_data = getFieldsetElementValue(fieldsetId, fieldsetElementId, "paragraph_data");

                    _HTML += "<div class=\"fieldsetElement\">" + paragraph_data + "</div>";

                    break;
                case "checkbox":
                    string checkbox_label = getFieldsetElementValue(fieldsetId, fieldsetElementId, "checkbox_label");
                    bool checkbox_bold = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "checkbox_bold") == "true") ? true : false;
                    bool checkbox_italic = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "checkbox_italic") == "true") ? true : false;
                    bool checkbox_validation_required = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "checkbox_validation_required") == "true") ? true : false;
                    string checkbox_validation_required_error_message = Languages.GetLanguageString("config/display/FormValidationErrorMessages/RequiredField/language", this._FullLanguageName, "content", this._ViewType);

                    _HTML += "<div><input type=\"hidden\" id=\"checkbox_label_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"checkbox_label_" + fieldsetId + "_" + fieldsetElementId + "\" value=\"" + checkbox_label + "\" /></div>";

                    _HTML += "<div class=\"fieldsetElement\">";

                    _HTML += "<label>";
                    _HTML += getLabel(checkbox_label, checkbox_bold, checkbox_italic, checkbox_validation_required);
                    _HTML += "</label>";

                    _HTML += "<div class=\"group\">"; 

                    _HTML += "<CustomControls2:CustomCheckBoxList id=\"checkbox_" + fieldsetId + "_" + fieldsetElementId + "\" runat=\"server\" RepeatLayout=\"Flow\" >";

                    //RepeatColumns="2"
                    //RepeatDirection="Vertical/Horizontal"
                    //RepeatLayout="Flow/Table"

                    for (int i = 0; i < _formElements.Length; i++)
                    {
                        if ((int.Parse(getFieldset(_formElements[i])) == fieldsetId) && (int.Parse(getFieldsetElement(_formElements[i])) == fieldsetElementId))
                        {
                            if (getFieldsetElementKey(_formElements[i]).IndexOf("_group_") != -1)
                            {
                                switch (getFieldsetElementGroupType(getFieldsetElementKey(_formElements[i])))
                                {
                                    case "item":
                                        _HTML += "<asp:ListItem class=\"checkbox\" Text=\"" + getFieldsetElementValue(_formElements[i]) + "\"";
                                        break;
                                    case "selected":
                                        if (getFieldsetElementValue(_formElements[i]) == "true")
                                        {
                                            _HTML += " Selected=\"True\"";
                                        }
                                        else
                                        {
                                            _HTML += " Selected=\"False\"";
                                        }
                                        _HTML += " />";
                                        break;
                                }
                            }
                        }
                    }

                    _HTML += "</CustomControls2:CustomCheckBoxList>";

                    if (checkbox_validation_required)
                    {
                        _FieldValidationExists = true;
                        _HTML += "<CustomControls2:RequiredFieldValidatorForCheckBoxLists runat=\"server\" ControlToValidate=\"checkbox_" + fieldsetId + "_" + fieldsetElementId + "\" ErrorMessage=\" " + checkbox_validation_required_error_message + "\" Display=\"Dynamic\" />";
                    }

                    _HTML += "</div>";
                    _HTML += "<div class=\"floatfix\"></div>";
                    _HTML += "</div>";

                    break;
                case "radio":
                    string radio_label = getFieldsetElementValue(fieldsetId, fieldsetElementId, "radio_label");
                    string radio_name = getFieldsetElementValue(fieldsetId, fieldsetElementId, "radio_name");
                    
                    bool radio_bold = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "radio_bold") == "true") ? true : false;
                    bool radio_italic = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "radio_italic") == "true") ? true : false;
                    bool radio_validation_required = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "radio_validation_required") == "true") ? true : false;
                    
                    string radio_validation_required_error_message = Languages.GetLanguageString("config/display/FormValidationErrorMessages/RequiredField/language", this._FullLanguageName, "content", this._ViewType);

                    _HTML += "<div><input type=\"hidden\" id=\"radio_label_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"radio_label_" + fieldsetId + "_" + fieldsetElementId + "\" value=\"" + radio_label + "\" /></div>";

                    _HTML += "<div class=\"fieldsetElement\">";
                    
                    _HTML += "<label>";
                    _HTML += getLabel(radio_label, radio_bold, radio_italic, radio_validation_required);
                    _HTML += "</label>";

                    _HTML += "<div class=\"group\">";

                    _HTML += "<CustomControls2:CustomRadioButtonList id=\"radio_" + fieldsetId + "_" + fieldsetElementId + "\" runat=\"server\" RepeatLayout=\"Flow\">";

                    //RepeatColumns="2"
                    //RepeatDirection="Vertical/Horizontal"
                    //RepeatLayout="Flow/Table"

                    for (int i = 0; i < _formElements.Length; i++)
                    {
                        if ((int.Parse(getFieldset(_formElements[i])) == fieldsetId) && (int.Parse(getFieldsetElement(_formElements[i])) == fieldsetElementId))
                        {
                            if (getFieldsetElementKey(_formElements[i]).IndexOf("_group_") != -1)
                            {
                                switch (getFieldsetElementGroupType(getFieldsetElementKey(_formElements[i])))
                                {
                                    case "item":
                                        _HTML += "<asp:ListItem class=\"radio\" Text=\"" + getFieldsetElementValue(_formElements[i]) + "\"";
                                        break;
                                    case "selected":
                                        if (getFieldsetElementValue(_formElements[i]) == "true")
                                        {
                                            _HTML += " Selected=\"True\"";
                                        }
                                        else
                                        {
                                            _HTML += " Selected=\"False\"";
                                        }
                                        _HTML += " />";
                                        break;
                                }
                            }
                        }
                    }

                    _HTML += "</CustomControls2:CustomRadioButtonList>";

                    if (radio_validation_required)
                    {
                        _FieldValidationExists = true;
                        _HTML += "<asp:RequiredFieldValidator runat=\"server\" ControlToValidate=\"radio_" + fieldsetId + "_" + fieldsetElementId + "\" ErrorMessage=\" " + radio_validation_required_error_message + "\" display=\"Dynamic\" />";
                    }

                    _HTML += "</div>";
                    _HTML += "<div class=\"floatfix\"></div>";
                    _HTML += "</div>";

                    break;
                case "select":
                    string select_label = getFieldsetElementValue(fieldsetId, fieldsetElementId, "select_label");
                    bool select_bold = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "select_bold") == "true") ? true : false;
                    bool select_italic = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "select_italic") == "true") ? true : false;
                    bool select_validation_required = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "select_validation_required") == "true") ? true : false;
                    string select_validation_required_error_message = Languages.GetLanguageString("config/display/FormValidationErrorMessages/RequiredField/language", this._FullLanguageName, "content", this._ViewType);
                    bool select_multiple = (getFieldsetElementValue(fieldsetId, fieldsetElementId, "select_multiple") == "true") ? true : false;
                    string select_size = getFieldsetElementValue(fieldsetId, fieldsetElementId, "select_size");

                    _HTML += "<div><input type=\"hidden\" id=\"select_label_" + fieldsetId + "_" + fieldsetElementId + "\" name=\"select_label_" + fieldsetId + "_" + fieldsetElementId + "\" value=\"" + select_label + "\" /></div>";

                    _HTML += "<div class=\"fieldsetElement\">";

                    _HTML += "<label for=\"select_" + fieldsetId + "_" + fieldsetElementId + "\">";
                    _HTML += getLabel(select_label, select_bold, select_italic, select_validation_required);
                    _HTML += "</label>";

                    _HTML += "<div class=\"group\">";

                    if (select_multiple)
                    {
                        _HTML += "<asp:ListBox id=\"select_" + fieldsetId + "_" + fieldsetElementId + "\" runat=\"server\" Rows=\"" + select_size + "\" SelectionMode=\"Multiple\">";

                    }
                    else
                    {
                        _HTML += "<asp:DropDownList id=\"select_" + fieldsetId + "_" + fieldsetElementId + "\" runat=\"server\">";
                    }

                    for (int i = 0; i < _formElements.Length; i++)
                    {
                        if ((int.Parse(getFieldset(_formElements[i])) == fieldsetId) && (int.Parse(getFieldsetElement(_formElements[i])) == fieldsetElementId))
                        {
                            if (getFieldsetElementKey(_formElements[i]).IndexOf("_group_") != -1)
                            {
                                switch (getFieldsetElementGroupType(getFieldsetElementKey(_formElements[i])))
                                {
                                    case "item":
                                        _HTML += "<asp:ListItem Text=\"" + getFieldsetElementValue(_formElements[i]) + "\"";
                                        break;
                                    case "selected":
                                        if (getFieldsetElementValue(_formElements[i]) == "true")
                                        {
                                            _HTML += " Selected=\"True\"";
                                        }
                                        else
                                        {
                                            _HTML += " Selected=\"False\"";
                                        }
                                        _HTML += " />";
                                        break;
                                }
                            }
                        }
                    }

                    if (select_multiple)
                    {
                        _HTML += "</asp:ListBox><br />";

                    }
                    else
                    {
                        _HTML += "</asp:DropDownList><br />";
                    }

                    if (select_validation_required)
                    {
                        _FieldValidationExists = true;
                        _HTML += "<asp:RequiredFieldValidator runat=\"server\" ControlToValidate=\"select_" + fieldsetId + "_" + fieldsetElementId + "\" ErrorMessage=\" " + select_validation_required_error_message + "\" display=\"Dynamic\" />";
                    }

                    _HTML += "</div>";
                    _HTML += "<div class=\"floatfix\"></div>";
                    _HTML += "</div>";

                    break;
            }
        }

        private void checkRequiredField(int fieldsetId, int fieldsetElementId)
        {
            switch (getFieldsetElementType(fieldsetId, fieldsetElementId))
            {
                case "text":
                    string text_validation_required = getFieldsetElementValue(fieldsetId, fieldsetElementId, "text_validation_required");
                    if (text_validation_required == "true")
                    {
                        _FieldValidationExists = true;
                    }
                    break;

                case "password":
                    string password_validation_required = getFieldsetElementValue(fieldsetId, fieldsetElementId, "password_validation_required");
                    if (password_validation_required == "true")
                    {
                        _FieldValidationExists = true;
                    }
                    break;

                case "textarea":
                    string textarea_validation_required = getFieldsetElementValue(fieldsetId, fieldsetElementId, "textarea_validation_required");
                    if (textarea_validation_required == "true")
                    {
                        _FieldValidationExists = true;
                    }
                    break;

                case "checkbox":
                    string checkbox_validation_required = getFieldsetElementValue(fieldsetId, fieldsetElementId, "checkbox_validation_required");
                    if (checkbox_validation_required == "true")
                    {
                        _FieldValidationExists = true;
                    }
                    break;

                case "radio":
                    string radio_validation_required = getFieldsetElementValue(fieldsetId, fieldsetElementId, "radio_validation_required");
                    if (radio_validation_required == "true")
                    {
                        _FieldValidationExists = true;
                    }
                    break;

                case "select":
                    string select_validation_required = getFieldsetElementValue(fieldsetId, fieldsetElementId, "select_validation_required");
                    if (select_validation_required == "true")
                    {
                        _FieldValidationExists = true;
                    }
                    break;
            }
        }

        private string getFieldset(String arrayFieldset)
        {
            return arrayFieldset.Substring(1, arrayFieldset.IndexOf("][") - 1);
        }

        private string getFieldsetElement(String arrayElement)
        {
            string tmp = arrayElement.Substring(arrayElement.IndexOf("][") + 2, arrayElement.Length - (arrayElement.IndexOf("][") + 2));
            return tmp.Substring(0, tmp.IndexOf("]["));
        }

        private string getFieldsetElementKey(String arrayElementKey)
        {
            string tmp = arrayElementKey.Substring(arrayElementKey.IndexOf("][") + 2, arrayElementKey.Length - (arrayElementKey.IndexOf("][") + 2));
            return tmp.Substring(tmp.IndexOf("][") + 2, (tmp.IndexOf("]=[") - tmp.IndexOf("][") - 2));
        }

        private string getFieldsetElementValue(string arrayElementValue)
        {
            return arrayElementValue.Substring((arrayElementValue.LastIndexOf("]=[") + 3), (arrayElementValue.Length - 1) - (arrayElementValue.LastIndexOf("]=[") + 3));
        }

        private string getFieldsetElementValue(int fieldset, int fieldsetElement, string fieldsetElementKey)
        {
            string fieldsetElementValue = "";
            for (int i = 0; i < _formElements.Length; i++)
            {
                if (int.Parse(getFieldset(_formElements[i])) == fieldset)
                {
                    if (int.Parse(getFieldsetElement(_formElements[i])) == fieldsetElement)
                    {
                        if (getFieldsetElementKey(_formElements[i]) == fieldsetElementKey)
                        {
                            fieldsetElementValue = getFieldsetElementValue(_formElements[i]);
                        }
                    }
                }
            }
            return fieldsetElementValue;
        }

        private string getFieldsetElementType(int fieldset, int fieldsetElement)
        {
            string fieldsetElementType = "";
            for (int i = 0; i < _formElements.Length; i++)
            {
                if (int.Parse(getFieldset(_formElements[i])) == fieldset)
                {
                    if (int.Parse(getFieldsetElement(_formElements[i])) == fieldsetElement)
                    {
                        fieldsetElementType = getFieldsetElementKey(_formElements[i]);
                        fieldsetElementType = fieldsetElementType.Substring(0, fieldsetElementType.IndexOf("_"));
                    }
                }
            }
            return fieldsetElementType;
        }

        private string getFieldsetElementGroupType(String fieldsetElementKey)
        {
            string fieldsetElementGroupType = fieldsetElementKey.Substring(fieldsetElementKey.IndexOf("_group_") + 7, fieldsetElementKey.Length - (fieldsetElementKey.IndexOf("_group_") + 7));

            return fieldsetElementGroupType;
        }

        private string getLabel(string label, bool bold, bool italic, bool required)
        {
            StringBuilder _HTML = new StringBuilder();
            
            _HTML.Append((bold) ? "<strong>" : "");
            _HTML.Append((italic) ? "<em>" : "");
            
            _HTML.Append(label);
            
            _HTML.Append((required) ? "<span class=\"warning\"> *</span>" : "");

            _HTML.Append((italic) ? "</em>" : "");
            _HTML.Append((bold) ? "</strong>" : "");
            
            return _HTML.ToString();
        }
    }
}
