using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Modules.PhotoAlbum.Controls
{

  /// <summary>
  /// Repeater with support for DataPager
  /// </summary>
  [ToolboxData("<{0}:DataPagerRepeater runat=server PersistentDataSource=true></{0}:DataPagerRepeater>")]
  public class DataPagerRepeater : Repeater, System.Web.UI.WebControls.IPageableItemContainer, INamingContainer
  {
    /// <summary>
    /// Number of rows to show
    /// </summary>
    public int MaximumRows
    {
      get { return ViewState["MaximumRows"] != null ? (int) ViewState["MaximumRows"] : -1; }
    }

    /// <summary>
    /// First row to show
    /// </summary>
    public int StartRowIndex
    {
      get { return ViewState["StartRowIndex"] != null ? (int) ViewState["StartRowIndex"] : -1; }
    }

    /// <summary>
    /// Total rows. When PagingInDataSource is set to true you must get the total records from the datasource (without paging) at the FetchingData event
    /// When PagingInDataSource is set to true you also need to set this when you load the data the first time.
    /// </summary>
    public int TotalRows
    {
      get { return ViewState["TotalRows"] != null ? (int) ViewState["TotalRows"] : -1; }
      set { ViewState["TotalRows"] = value; }
    }

    /// <summary>
    /// If repeater should store data source in view state. If false you need to get and bind data at post back. When using a connected data source this is handled by the data source.  
    /// </summary>        
    public bool PersistentDataSource
    {
      get { return ViewState["PersistentDataSource"] != null ? (bool) ViewState["PersistentDataSource"] : true; }
      set { ViewState["PersistentDataSource"] = value; }
    }

    /// <summary>
    /// Set to true if you want to handle paging in the data source. 
    /// Ex if you are selecting data from the database and only select the current rows 
    /// you must set this property to true and get and rebind data at the FetchingData event. 
    /// If this is true you must also set the TotalRecords property at the FetchingData event.     
    /// </summary>
    /// <seealso cref="FetchingData"/>
    /// <seealso cref="TotalRows"/>
    public bool PagingInDataSource
    {
      get { return ViewState["PageingInDataSource"] != null ? (bool) ViewState["PageingInDataSource"] : false; }
      set { ViewState["PageingInDataSource"] = value; }
    }

    /// <summary>
    /// Checks if you need to rebind data source at postback
    /// </summary>
    public bool NeedsDataSource
    {
      get
      {
        if (PagingInDataSource)
          return true;
        if (IsBoundUsingDataSourceID == false && !Page.IsPostBack)
          return true;
        if (IsBoundUsingDataSourceID == false && PersistentDataSource == false && Page.IsPostBack)
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Loading ViewState
    /// </summary>
    /// <param name="savedState"></param>
    protected override void LoadViewState(object savedState)
    {
      base.LoadViewState(savedState);
    }

    protected override void OnLoad(System.EventArgs e)
    {
      if (Page.IsPostBack)
      {
        if (NeedsDataSource && FetchingData != null)
        {
          if (PagingInDataSource)
          {
            SetPageProperties(StartRowIndex, MaximumRows, true);
          }
          FetchingData(this, null);
        }

        if (!IsBoundUsingDataSourceID && PersistentDataSource && ViewState["DataSource"] != null)
        {
          this.DataSource = ViewState["DataSource"];
          this.DataBind();
        }
        if (IsBoundUsingDataSourceID)
        {
          this.DataBind();
        }
      }

      base.OnLoad(e);
    }

    /// <summary>
    /// Method used by pager to set totalrecords
    /// </summary>
    /// <param name="startRowIndex">startRowIndex</param>
    /// <param name="maximumRows">maximumRows</param>
    /// <param name="databind">databind</param>
    public void SetPageProperties(int startRowIndex, int maximumRows, bool databind)
    {
      ViewState["StartRowIndex"] = startRowIndex;
      ViewState["MaximumRows"] = maximumRows;
      if (TotalRows > -1)
      {
        if (TotalRowCountAvailable != null)
        {
          TotalRowCountAvailable(this,
                                 new PageEventArgs((int) ViewState["StartRowIndex"], (int) ViewState["MaximumRows"],
                                                   TotalRows));
        }
      }
    }

    /// <summary>
    /// OnDataPropertyChanged
    /// </summary>
    protected override void OnDataPropertyChanged()
    {
      if (MaximumRows != -1 || IsBoundUsingDataSourceID)
      {
        this.RequiresDataBinding = true;
      }
      base.OnDataPropertyChanged();
    }

    /// <summary>
    /// Renders only current items selected by pager
    /// </summary>
    /// <param name="writer"></param>
    protected override void RenderChildren(HtmlTextWriter writer)
    {
      if (!PagingInDataSource && MaximumRows != -1)
      {
        foreach (RepeaterItem item in this.Items)
        {
          if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
          {
            item.Visible = false;
            if (item.ItemIndex >= (int) ViewState["StartRowIndex"] &&
                item.ItemIndex < ((int) ViewState["StartRowIndex"] + (int) ViewState["MaximumRows"]))
            {
              item.Visible = true;
            }
          }
          else
          {
            item.Visible = true;
          }
        }
      }
      base.RenderChildren(writer);
    }

    /// <summary>
    /// Get Data
    /// </summary>
    /// <returns></returns>
    protected override System.Collections.IEnumerable GetData()
    {
      System.Collections.IEnumerable dataObjects = base.GetData();

      if (dataObjects == null && this.DataSource != null)
      {
        if (this.DataSource is System.Collections.IEnumerable)
          dataObjects = (System.Collections.IEnumerable) this.DataSource;
        else
          dataObjects = ((System.ComponentModel.IListSource) this.DataSource).GetList();
      }

      if (!PagingInDataSource && MaximumRows != -1 && dataObjects != null)
      {
        int i = -1;

        if (dataObjects != null)
        {
          i = 0;
          foreach (object o in dataObjects)
          {
            i++;
          }

        }

        ViewState["TotalRows"] = i;

        if (!IsBoundUsingDataSourceID && PersistentDataSource)
          ViewState["DataSource"] = this.DataSource;

        SetPageProperties(StartRowIndex, MaximumRows, true);
      }

      if (PagingInDataSource && !Page.IsPostBack)
      {
        SetPageProperties(StartRowIndex, MaximumRows, true);
      }

      return dataObjects;
    }


    /// <summary>
    /// Event when pager/repeater have counted total rows
    /// </summary>
    public event System.EventHandler<PageEventArgs> TotalRowCountAvailable;

    /// <summary>
    /// Event when repeater gets the data on postback
    /// </summary>
    public event System.EventHandler<PageEventArgs> FetchingData;

  }
}
