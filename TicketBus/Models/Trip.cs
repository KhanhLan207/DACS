using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketBus.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }

        public int RouteId { get; set; }
        
        [ForeignKey("RouteId")]
        public BusRoute Route { get; set; }
        
        public DateTime DepartureDate { get; set; }
        
        public TimeSpan DepartureTime { get; set; }
        
        public int CoachId { get; set; }
        
        [ForeignKey("CoachId")]
        public Coach Coach { get; set; }
        
        public TripStatus Status { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
        
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
    
    public enum TripStatus
    {
        Scheduled = 0,
        Departed = 1,
        Completed = 2,
        Cancelled = 3
    }
} 