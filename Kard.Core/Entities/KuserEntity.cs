using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:user
        public class KuserEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public KuserEntity()
        {
        
        }
        
    
        public int Id{get; set;}  
          public string Name{get; set;}  
          public string Phone{get; set;}  
          public string Email{get; set;}  
          public string Password{get; set;}  
          public string NikeName{get; set;}  
          public string PhotoPath{get; set;}  
          public string CoverPath{get; set;}  
          public string Sex{get; set;}  
          public int ExperienceValue{get; set;}  
          public int KroleId{get; set;}  
           
    }
    
}