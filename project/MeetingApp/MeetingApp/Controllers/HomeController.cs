using System.Diagnostics;
using MeetingApp.Models;
using MeetingApp.Models.SqlClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static MeetingApp.Models.SqlClasses.SQLEditMinute_Item_Status;

namespace MeetingApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        // Create lists to hold the meeting and meeting type data
        var meetingTypes = new List<MeetingType>();
        var meetings = new List<Meeting>();

        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

        meetingTypes = SqlDataCollection.getMeetingTypeList(connectionString, meetingTypes);
        meetings = SqlDataCollection.getMeetingList(connectionString, meetings); 
        var viewModel = new MeetingViewModel
        {
            MeetingTypes = meetingTypes,
            Meetings = meetings
        };

        return View(viewModel);
    }

    public IActionResult Delete(int id)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        SqlProcessing.DeleteMeeting(connectionString, id); 
        return RedirectToAction("Index");
    }
    public IActionResult DeleteMeetingItem(int id, Minutes_Item model)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        SqlProcessing.DeleteMinutes_Item(connectionString, id);
        return RedirectToAction("View", new { id = model.meetingId, description = model.MeetingDescription, createdDate = model.MeetingCreatedDate, meetingNumber = model.meetingNumber });
    }


    public IActionResult View(int id, string description, DateTime createdDate, string meetingNumber)
    {

        var minuteItemStatuses = new List<Minute_Item_Status>();
        var statuses = new List<Status>();
        var minutesItems = new List<Minutes_Item>();
        var employees = new List<Employee>();

        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        statuses = SqlDataCollection.getStatusList(connectionString, statuses);
        minuteItemStatuses = SqlDataCollection.getMinute_Item_StatusList(connectionString, minuteItemStatuses);
        employees = SqlDataCollection.getEmployeeList(connectionString, employees);
        minutesItems = SqlDataCollection.getMinutes_ItemList(connectionString, minutesItems); 
        var viewModel = new DisplayMeeting
        {
            MeetingId = id,
            MeetingTypeDescription = description,
            Meeting_date_created = createdDate,
            MeetingNumber = meetingNumber,
            Minute_Item_Status_List = minuteItemStatuses,
            StatusesList = statuses,
            Minutes_Item_List = minutesItems,
            EmployeesList = employees
        };
        return View(viewModel);
    }




    public IActionResult EditMinuteItem(int id, int meetingId, string meetingTypeDescription, DateTime meetingDateCreated, string meetingNumber)
    {
        Minutes_Item? minuteItem = null; // Assuming 'MinuteItem' is your model for the Minutes_Item table
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        minuteItem =  SqlDataCollection.getMinutes_ItemByID(connectionString, id , minuteItem); 
        minuteItem.meetingNumber = meetingNumber;
        minuteItem.meetingId = meetingId;
        minuteItem.MeetingDescription = meetingTypeDescription;
        minuteItem.MeetingCreatedDate = meetingDateCreated;

        return View(minuteItem);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditMinuteItem(int id, Minutes_Item model)
    {
        if (id != model.MinutesItemID)
        {
            return BadRequest();
        }

        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        try
        {
            SQLCreateMinuteItemProcess.UpdateMinuteItem(connectionString, id, model);
            return RedirectToAction("View", new { id = model.meetingId, description = model.MeetingDescription, createdDate = model.MeetingCreatedDate, meetingNumber = model.meetingNumber });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }


    public IActionResult CreateMinuteItem(int MeetingId, string meetingTypeDescription, string meetingNumber, DateTime meetingDateCreated)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;


        var viewModel = new CreateMinuteItem
        {
            EmployeesList = new List<Employee>(),
            StatusesList = new List<Status>(),
            MeetingId = MeetingId,
            MeetingTypeDescription = meetingTypeDescription,
            MeetingDateCreated = meetingDateCreated,
            MeetingNumber = meetingNumber
        };

        viewModel.EmployeesList = SqlDataCollection.getEmployeeList(connectionString, viewModel.EmployeesList);
        viewModel.StatusesList = SqlDataCollection.getStatusList(connectionString, viewModel.StatusesList);
        return View(viewModel);
    }
    [HttpPost]
    public IActionResult CreateMinuteItem(CreateMinuteItem model)
    {
        if (ModelState.IsValid)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SQLCreateMinuteItemProcess.InsertMinutes_ItemANDMinute_Item_Status(connectionString, model);
            string? redirectUrl = Url.Action("View", new
            {
                id = model.MeetingId,
                description = model.MeetingTypeDescription,
                createdDate = model.MeetingDateCreated,
                meetingNumber = model.MeetingNumber
            });
            return Redirect(redirectUrl);
        }
        return View(model);
    }

    public IActionResult CreateMeeting()
    {
        var meetingTypes = new List<MeetingType>();
        var minuteItemStatuses = new List<Minute_Item_Status>();
        var minutesItems = new List<Minutes_Item>();

        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        minutesItems = SqlDataCollection.getMinutes_ItemList(connectionString, minutesItems);
        meetingTypes = SqlDataCollection.getMeetingTypeList(connectionString, meetingTypes);
        minuteItemStatuses = SqlDataCollection.getMinute_Item_StatusList(connectionString, minuteItemStatuses); 
      
        var meetingViewModel = new Meeting
        {
            MeetingTypeList = meetingTypes,
            Minute_Item_StatusList = minuteItemStatuses,
            MinutesList = minutesItems
        };

        return View(meetingViewModel);  // Pass the view model to the view
    }

    [HttpPost]
    public IActionResult CreateMeeting(Meeting model)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        if (model.MinutesIDList.Count == 0)
        {
            SqlProcessing.InsertNewMeeting(connectionString, model);
            return RedirectToAction("Index", "Home");
        }
        List<int> meetingIds = SqlProcessing.getMeetingIdsList(connectionString, model);
        List<int> meetingIds1 = SqlProcessing.getMeetingIdsListByMinuteID(connectionString, model);
        List<int> commonMeetingIds = meetingIds.Intersect(meetingIds1).ToList();

        string commonMeetingIdsStr = string.Join(",", commonMeetingIds);
        string minutesIDListStr = string.Join(",", model.MinutesIDList);
        var minuteItemStatusList = SqlProcessing.getMinute_Item_StatusBYmeeting_IdANDminutes_ItemID(connectionString, commonMeetingIdsStr, minutesIDListStr);
        SqlProcessing.InsertIntoMeetingANDMinuteItemStatus(connectionString, model, minuteItemStatusList);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult GetMinutesItemsByMeetingType(int meetingTypeId)
    {
        var minute_Items = new List<Minutes_Item>();
        var minuteItemStatuses = new List<Minute_Item_Status>();
        List<int> meetingIds = new List<int>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        meetingIds= SQLGetMinutesItemsByMeetingType.getMeetingIds(connectionString, meetingTypeId);
        minuteItemStatuses = SQLGetMinutesItemsByMeetingType.getMinuteItemStatusesList(connectionString, meetingIds,minuteItemStatuses);
        minute_Items = SQLGetMinutesItemsByMeetingType.getMinute_ItemsList(connectionString,minuteItemStatuses,minute_Items);
        return Json(minute_Items);
    }


    public IActionResult EditMeetingStatus()
    {
        var meetingList = new List<Meeting>();
        var EmployeeList = new List<Employee>();
        var minutes_ItemList = new List<Minutes_Item>();
        var minutes_Item_StatusList = new List<Minute_Item_Status>();
        var statusList = new List<Status>();
        var Minute_Item_StatusObj = new Minute_Item_Status();

        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        EmployeeList = SqlDataCollection.getEmployeeList(connectionString, EmployeeList);
        meetingList = SqlDataCollection.getMeetingList(connectionString, meetingList);
        statusList = SqlDataCollection.getStatusList(connectionString, statusList);
        minutes_ItemList = SqlDataCollection.getMinutes_ItemList(connectionString, minutes_ItemList); 

        Minute_Item_StatusObj.MeetingList = meetingList;
        Minute_Item_StatusObj.EmployeeList = EmployeeList;
        Minute_Item_StatusObj.StatusList = statusList;
        Minute_Item_StatusObj.Minute_Item_StatusList = minutes_Item_StatusList;
        Minute_Item_StatusObj.MinutesList = minutes_ItemList;
        return View(Minute_Item_StatusObj);
    }

    [HttpPost]
    public IActionResult EditMeetingStatus(Minute_Item_Status model, int EmployeeID, int MinutesItemID, int MeetingId, DateTime DueDate, DateTime CompletedDate, string Action , int StatusID)
    {
       
        string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
       var result =  SQLEditMinute_Item_Status.checkRecords(connectionString, MeetingId, MinutesItemID, model, DueDate, CompletedDate, Action, StatusID);

        if (result == CheckRecordResult.NotFound)
        {
            TempData["ErrorMessage"] = "The specified minute item status was not found. Please enter the correct information";
            return RedirectToAction("EditMeetingStatus");
        }
        else
        {
            return RedirectToAction("Index");

        }   
   
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
