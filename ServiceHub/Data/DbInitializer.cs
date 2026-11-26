using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Models;

namespace ServiceHub.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // ── ROLES ────────────────────────────────────────────────────────────
            foreach (var role in new[] { "Admin", "ServiceProvider", "Customer" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── CATEGORIES ───────────────────────────────────────────────────────
            var categoriesToSeed = new[]
            {
                new Category { Name = "Electrical", Description = "Professional electrical installation, wiring, smart home automation, and repairs.", IconClass = "fas fa-bolt", ImageUrl = "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?w=800&q=80", IsActive = true },
                new Category { Name = "Plumbing", Description = "Expert plumbing solutions including leak detection, pipe fitting, and pump repair.", IconClass = "fas fa-faucet", ImageUrl = "https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?w=800&q=80", IsActive = true },
                new Category { Name = "Home Cleaning", Description = "Thorough home, kitchen, sofa shampooing, and post-construction cleaning.", IconClass = "fas fa-broom", ImageUrl = "https://images.unsplash.com/photo-1581578731548-c64695cc6952?w=800&q=80", IsActive = true },
                new Category { Name = "Tutoring", Description = "Academic coaching, board exam prep, Python programming, and personal fitness.", IconClass = "fas fa-graduation-cap", ImageUrl = "https://images.unsplash.com/photo-1580582932707-520aed937b7b?w=800&q=80", IsActive = true },
                new Category { Name = "Appliance Repair", Description = "Expert repair for AC, refrigerators, washing machines, RO purifiers, and ovens.", IconClass = "fas fa-blender", ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&q=80", IsActive = true },
                new Category { Name = "Painting & Renovation", Description = "Interior wall painting, Dr. Fixit waterproofing, and designer texture walls.", IconClass = "fas fa-paint-roller", ImageUrl = "https://images.unsplash.com/photo-1589939705384-5185137a7f0f?w=800&q=80", IsActive = true },
                new Category { Name = "Pest Control", Description = "Bayer odorless gel treatment, termite eradication, and mosquito fogging.", IconClass = "fas fa-bug", ImageUrl = "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=800&q=80", IsActive = true },
                new Category { Name = "Beauty & Salon", Description = "Hygienic at-home salon, herbal facials, bridal packages, and men's grooming.", IconClass = "fas fa-spa", ImageUrl = "https://images.unsplash.com/photo-1560750588-73207b1ef5b8?w=800&q=80", IsActive = true },
                new Category { Name = "Carpentry", Description = "Furniture assembly, lock repair, hydraulic fittings, and modular kitchen repair.", IconClass = "fas fa-hammer", ImageUrl = "https://images.unsplash.com/photo-1538688525198-9b88f6f53126?w=800&q=80", IsActive = true },
                new Category { Name = "Gardening & Landscaping", Description = "Urban terrace garden setup, lawn mowing, tree pruning, and plant care.", IconClass = "fas fa-seedling", ImageUrl = "https://images.unsplash.com/photo-1585320806297-9794b3e4eeae?w=800&q=80", IsActive = true }
            };

            foreach (var cat in categoriesToSeed)
            {
                var existing = await context.Categories.FirstOrDefaultAsync(c => c.Name == cat.Name);
                if (existing == null)
                {
                    context.Categories.Add(cat);
                }
                else
                {
                    existing.Description = cat.Description;
                    existing.IconClass = cat.IconClass;
                    existing.ImageUrl = cat.ImageUrl;
                    existing.IsActive = true;
                }
            }
            await context.SaveChangesAsync();

            // ── ADMIN ─────────────────────────────────────────────────────────────
            if (await userManager.FindByEmailAsync("admin@servicehub.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@servicehub.com",
                    Email = "admin@servicehub.com",
                    FullName = "Rajesh Sharma",
                    EmailConfirmed = true,
                    PhoneNumber = "+91 98200 99887",
                    City = "Mumbai",
                    Address = "ServiceHub HQ, Bandra Kurla Complex, Mumbai",
                    ProfilePictureUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200&q=80",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };
                var r = await userManager.CreateAsync(admin, "Admin@123!");
                if (r.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ── HELPER: Ensure Provider ───────────────────────────────────────────
            async Task<ApplicationUser> EnsureProvider(string email, string fullName, string phone, string city, string address, string pic)
            {
                var existing = await userManager.FindByEmailAsync(email);
                if (existing != null)
                {
                    existing.FullName = fullName;
                    existing.PhoneNumber = phone;
                    existing.City = city;
                    existing.Address = address;
                    existing.ProfilePictureUrl = pic;
                    existing.IsVerified = true;
                    await userManager.UpdateAsync(existing);
                    return existing;
                }

                var u = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    PhoneNumber = phone,
                    City = city,
                    Address = address,
                    ProfilePictureUrl = pic,
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };
                var r = await userManager.CreateAsync(u, "Provider@123!");
                if (r.Succeeded)
                {
                    await userManager.AddToRoleAsync(u, "ServiceProvider");
                }
                return u;
            }

            // ── HELPER: Ensure Service ────────────────────────────────────────────
            async Task<Service> EnsureService(Service s)
            {
                var existing = await context.Services.FirstOrDefaultAsync(x => x.Title == s.Title && x.ProviderId == s.ProviderId);
                if (existing != null)
                {
                    existing.Price = s.Price;
                    existing.PriceType = s.PriceType;
                    existing.Description = s.Description;
                    existing.City = s.City;
                    existing.ServiceArea = s.ServiceArea;
                    existing.ImageUrl = s.ImageUrl;
                    existing.IsActive = true;
                    await context.SaveChangesAsync();
                    return existing;
                }

                context.Services.Add(s);
                await context.SaveChangesAsync();
                return s;
            }

            // Category references
            var catElec = await context.Categories.FirstAsync(c => c.Name == "Electrical");
            var catPlumb = await context.Categories.FirstAsync(c => c.Name == "Plumbing");
            var catClean = await context.Categories.FirstAsync(c => c.Name == "Home Cleaning");
            var catTutor = await context.Categories.FirstAsync(c => c.Name == "Tutoring");
            var catApp = await context.Categories.FirstAsync(c => c.Name == "Appliance Repair");
            var catPaint = await context.Categories.FirstAsync(c => c.Name == "Painting & Renovation");
            var catPest = await context.Categories.FirstAsync(c => c.Name == "Pest Control");
            var catSalon = await context.Categories.FirstAsync(c => c.Name == "Beauty & Salon");
            var catCarp = await context.Categories.FirstAsync(c => c.Name == "Carpentry");
            var catGarden = await context.Categories.FirstAsync(c => c.Name == "Gardening & Landscaping");

            // ── 1. Ramesh Kumar — Senior Electrician (Mumbai)
            var p1 = await EnsureProvider("ramesh.electrical@servicehub.com", "Ramesh Kumar", "+91 98200 11234", "Mumbai", "14B, Andheri West, Mumbai", "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200&q=80");
            var s1 = await EnsureService(new Service { ProviderId = p1.Id, CategoryId = catElec.Id, Title = "Fan & Light Installation", Description = "Professional installation of ceiling fans, LED panels, chandeliers, and decorative lights with safe anchor fitting.", Price = 349m, PriceType = PriceType.Fixed, City = "Mumbai", ServiceArea = "Andheri, Bandra, Juhu, Malad", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });
            var s2 = await EnsureService(new Service { ProviderId = p1.Id, CategoryId = catElec.Id, Title = "Switchboard & Fuse Repair", Description = "Fast repair of short-circuiting switchboards, MCB panels, fuse boxes, and tripping breakers. Same-day emergency service.", Price = 499m, PriceType = PriceType.Fixed, City = "Mumbai", ServiceArea = "Andheri, Bandra, Juhu, Borivali", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(7, 0, 0), WorkEndTime = new TimeSpan(20, 0, 0) });
            var s3 = await EnsureService(new Service { ProviderId = p1.Id, CategoryId = catElec.Id, Title = "Home Rewiring & Panel Upgrade", Description = "Complete residential rewiring with heat-resistant copper cables and ISI distribution box installation.", Price = 4999m, PriceType = PriceType.Fixed, City = "Mumbai", ServiceArea = "Mumbai Suburbs", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });
            var s4 = await EnsureService(new Service { ProviderId = p1.Id, CategoryId = catElec.Id, Title = "Inverter Setup & UPS Installation", Description = "Complete installation of home inverters (Luminous, Exide, Microtek), battery wiring, and main panel bypass connection.", Price = 799m, PriceType = PriceType.Fixed, City = "Mumbai", ServiceArea = "Andheri, Bandra, Borivali", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1473341304170-971dccb5ac1e?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });

            // ── 2. Amit Patel — Master Plumber (Delhi / Delhi NCR)
            var p2 = await EnsureProvider("amit.plumber@servicehub.com", "Amit Patel", "+91 98110 56789", "Delhi NCR", "22, Rohini Sector 11, Delhi", "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=200&q=80");
            var s5 = await EnsureService(new Service { ProviderId = p2.Id, CategoryId = catPlumb.Id, Title = "Pipe Leakage Repair", Description = "Rapid detection and repair of concealed pipe leaks, joint seepages, and broken PVC/CPVC/GI pipelines.", Price = 399m, PriceType = PriceType.Fixed, City = "Delhi NCR", ServiceArea = "Rohini, Pitampura, Dwarka, Janakpuri", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(7, 0, 0), WorkEndTime = new TimeSpan(20, 0, 0) });
            var s6 = await EnsureService(new Service { ProviderId = p2.Id, CategoryId = catPlumb.Id, Title = "Tap & Mixer Fitting", Description = "Fitting and replacement of kitchen sinks, quarter-turn taps, mixer faucets, and diverters. Jaquar, Kohler, Parryware.", Price = 299m, PriceType = PriceType.Fixed, City = "Delhi NCR", ServiceArea = "Delhi NCR", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1584813470613-5b1c1cad3d69?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });
            var s7 = await EnsureService(new Service { ProviderId = p2.Id, CategoryId = catPlumb.Id, Title = "Drain & Toilet Unclogging", Description = "High-pressure jetting and heavy-duty coil clearing for blocked kitchen sinks, bathroom floor traps, and commodes.", Price = 599m, PriceType = PriceType.Fixed, City = "Delhi NCR", ServiceArea = "Delhi NCR", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(6, 0, 0), WorkEndTime = new TimeSpan(21, 0, 0) });
            var s8 = await EnsureService(new Service { ProviderId = p2.Id, CategoryId = catPlumb.Id, Title = "Water Tank Cleaning", Description = "6-stage mechanized cleaning of overhead and underground water tanks with anti-bacterial UV treatment.", Price = 999m, PriceType = PriceType.Fixed, City = "Delhi NCR", ServiceArea = "Delhi NCR", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1564844536308-c5b5d6e9d1b7?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(7, 0, 0), WorkEndTime = new TimeSpan(17, 0, 0) });
            var s9 = await EnsureService(new Service { ProviderId = p2.Id, CategoryId = catPlumb.Id, Title = "Overhead Tank Valve Replacement", Description = "Heavy brass float valve replacement, automatic overflow cutoff ball, and inlet/outlet PPR joint repair.", Price = 449m, PriceType = PriceType.Fixed, City = "Delhi NCR", ServiceArea = "Delhi NCR", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });

            // ── 3. Priya Sharma — Deep Cleaning Specialist (Bengaluru)
            var p3 = await EnsureProvider("priya.clean@servicehub.com", "Priya Sharma", "+91 98450 98765", "Bengaluru", "3rd Cross, Indiranagar, Bengaluru", "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=200&q=80");
            var s10 = await EnsureService(new Service { ProviderId = p3.Id, CategoryId = catClean.Id, Title = "Full House Deep Cleaning", Description = "Comprehensive deep cleaning of 1BHK/2BHK/3BHK homes with industrial floor scrubber, glass polish, and balcony pressure wash.", Price = 2499m, PriceType = PriceType.Fixed, City = "Bengaluru", ServiceArea = "Indiranagar, Koramangala, HSR Layout, Whitefield", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1581578731548-c64695cc6952?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });
            var s11 = await EnsureService(new Service { ProviderId = p3.Id, CategoryId = catClean.Id, Title = "Full Kitchen Deep Cleaning", Description = "Intensive degreasing of chimney filters, exhaust fan, gas hob, modular trolleys inside-out, granite platform, and tile grout.", Price = 999m, PriceType = PriceType.Fixed, City = "Bengaluru", ServiceArea = "Indiranagar, Koramangala, HSR Layout", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(17, 0, 0) });
            var s12 = await EnsureService(new Service { ProviderId = p3.Id, CategoryId = catClean.Id, Title = "Sofa & Mattress Sanitization", Description = "Wet injection-extraction shampooing and UV allergen treatment for fabric sofas, recliner chairs, and king mattresses.", Price = 1299m, PriceType = PriceType.Fixed, City = "Bengaluru", ServiceArea = "Bengaluru", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&q=80", WorkDays = "Mon,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(17, 0, 0) });
            var s13 = await EnsureService(new Service { ProviderId = p3.Id, CategoryId = catClean.Id, Title = "Post-Construction Home Cleaning", Description = "Thorough removal of paint drops, cement residue, gypsum plaster dust, floor tile buffing, and window track cleaning.", Price = 4999m, PriceType = PriceType.Fixed, City = "Bengaluru", ServiceArea = "Bengaluru Urban", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1527515637462-cff94eecc1ac?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });

            // ── 4. Suresh Yadav — Academic & Exam Mentor (Pune)
            var p4 = await EnsureProvider("suresh.tutor@servicehub.com", "Suresh Yadav", "+91 98230 78901", "Pune", "Baner Road, Pune", "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=200&q=80");
            var s14 = await EnsureService(new Service { ProviderId = p4.Id, CategoryId = catTutor.Id, Title = "Class 10 & 12 Board Exam Tutor", Description = "Personalized CBSE and ICSE coaching in Mathematics and Physics. Past papers, conceptual drills, and weekly assessments.", Price = 800m, PriceType = PriceType.Hourly, City = "Pune", ServiceArea = "Baner, Aundh, Wakad, Hinjewadi", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1580582932707-520aed937b7b?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(16, 0, 0), WorkEndTime = new TimeSpan(21, 0, 0) });
            var s15 = await EnsureService(new Service { ProviderId = p4.Id, CategoryId = catTutor.Id, Title = "Class 1 to 10 All Subjects Tutor", Description = "Dedicated home tutor for school students in Maths, Science, Social Studies, and English with daily homework support.", Price = 650m, PriceType = PriceType.Hourly, City = "Pune", ServiceArea = "Baner, Aundh, Kothrud", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri", WorkStartTime = new TimeSpan(15, 0, 0), WorkEndTime = new TimeSpan(20, 0, 0) });
            var s16 = await EnsureService(new Service { ProviderId = p4.Id, CategoryId = catTutor.Id, Title = "Coding & Python Fundamentals Tutor", Description = "Learn programming from scratch: Python syntax, algorithms, data structures, and mini project building for school & college students.", Price = 900m, PriceType = PriceType.Hourly, City = "Pune", ServiceArea = "Pune / Online", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(10, 0, 0), WorkEndTime = new TimeSpan(20, 0, 0) });

            // ── 5. Rahul Mehta — Appliance Engineer (Hyderabad)
            var p5 = await EnsureProvider("rahul.appliance@servicehub.com", "Rahul Mehta", "+91 98490 76543", "Hyderabad", "Madhapur, Hyderabad", "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=200&q=80");
            var s17 = await EnsureService(new Service { ProviderId = p5.Id, CategoryId = catApp.Id, Title = "Fully Automatic Washing Machine Service", Description = "Comprehensive servicing of front-load & top-load washing machines. Fix spin vibration, drain pump issues, and motor drive.", Price = 699m, PriceType = PriceType.Fixed, City = "Hyderabad", ServiceArea = "Madhapur, Gachibowli, Kondapur, Jubilee Hills", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });
            var s18 = await EnsureService(new Service { ProviderId = p5.Id, CategoryId = catApp.Id, Title = "Split/Window AC Deep Gas Refilling", Description = "Complete AC gas charging (R32 / R410A / R22) with electronic nitrogen leak testing and condenser fin chemical wash.", Price = 1899m, PriceType = PriceType.Fixed, City = "Hyderabad", ServiceArea = "Hyderabad Metro", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1615648422001-14b4e1ae82ee?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(20, 0, 0) });
            var s19 = await EnsureService(new Service { ProviderId = p5.Id, CategoryId = catApp.Id, Title = "Inverter Battery Maintenance", Description = "Battery terminal cleaning, acid descaling, distilled water top-up, and load backup verification for home inverters.", Price = 399m, PriceType = PriceType.Fixed, City = "Hyderabad", ServiceArea = "Madhapur, Banjara Hills", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1473341304170-971dccb5ac1e?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });

            // ── 6. Bhavik Jadav — Electrical, Automation & AC Specialist (Rajkot)
            var p6 = await EnsureProvider("bhavik.rajkot@servicehub.com", "Bhavik Jadav", "+91 98250 87654", "Rajkot", "Yagnik Road, Rajkot", "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?w=200&q=80");
            var s20 = await EnsureService(new Service { ProviderId = p6.Id, CategoryId = catElec.Id, Title = "Smart Home Automation Setup", Description = "Convert your regular home into a smart home: smart Wi-Fi switchboards, Alexa/Google voice integration, and automated sensor lights.", Price = 5999m, PriceType = PriceType.Fixed, City = "Rajkot", ServiceArea = "Yagnik Road, Kalawad Road, University Road, 150ft Ring Road", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1558002038-1055907df827?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(19, 30, 0) });
            var s21 = await EnsureService(new Service { ProviderId = p6.Id, CategoryId = catElec.Id, Title = "Geyser Repair & Installation", Description = "Instant and storage water heater installation, replacement of burnt heating elements, thermostat check, and earth leakage protection.", Price = 549m, PriceType = PriceType.Fixed, City = "Rajkot", ServiceArea = "Rajkot City", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(20, 0, 0) });
            var s22 = await EnsureService(new Service { ProviderId = p6.Id, CategoryId = catElec.Id, Title = "CCTV Camera Fitting & Network Setup", Description = "HD night-vision IP CCTV camera mounting, 4/8 channel DVR setup, hard disk configuration, and mobile remote monitoring setup.", Price = 2999m, PriceType = PriceType.Fixed, City = "Rajkot", ServiceArea = "Rajkot", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1557597774-9d475d030a81?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });

            // ── 7. Dharmesh Parmar — RO & Water Tech Specialist (Ahmedabad)
            var p7 = await EnsureProvider("dharmesh.ro@servicehub.com", "Dharmesh Parmar", "+91 98790 34567", "Ahmedabad", "Maninagar, Ahmedabad", "https://images.unsplash.com/photo-1557862921-37829c790f19?w=200&q=80");
            var s23 = await EnsureService(new Service { ProviderId = p7.Id, CategoryId = catPlumb.Id, Title = "RO Water Purifier Servicing", Description = "Full service for Kent, Aquaguard, Pureit, and Livpure. Replacement of sediment filter, pre-carbon, RO membrane, and mineral cartridge.", Price = 499m, PriceType = PriceType.Fixed, City = "Ahmedabad", ServiceArea = "Maninagar, Vastrapur, Satellite, Bodakdev", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1564844536308-c5b5d6e9d1b7?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });
            var s24 = await EnsureService(new Service { ProviderId = p7.Id, CategoryId = catPlumb.Id, Title = "Submersible Pump Repair", Description = "Repair and rewinding of domestic borewell pumps, booster pumps, float switches, and starter panel capacitors.", Price = 1199m, PriceType = PriceType.Fixed, City = "Ahmedabad", ServiceArea = "Ahmedabad City", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });
            var s25 = await EnsureService(new Service { ProviderId = p7.Id, CategoryId = catPlumb.Id, Title = "Concealed Wall Leakage Detection", Description = "Advanced non-invasive electronic moisture mapping and acoustic leak detection for concealed bathroom and kitchen pipes.", Price = 799m, PriceType = PriceType.Fixed, City = "Ahmedabad", ServiceArea = "Ahmedabad & Gandhinagar", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });

            // ── 8. Karthik Rajan — Pest & Hygiene Expert (Chennai)
            var p8 = await EnsureProvider("karthik.pest@servicehub.com", "Karthik Rajan", "+91 98940 56789", "Chennai", "Anna Nagar, Chennai", "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=200&q=80");
            var s26 = await EnsureService(new Service { ProviderId = p8.Id, CategoryId = catPest.Id, Title = "Cockroach Control Gel Treatment", Description = "Odorless Bayer Maxforce gel dots placed in kitchen cabinets, fridge gaskets, and sink areas. 100% child and pet safe. 90-day guarantee.", Price = 699m, PriceType = PriceType.Fixed, City = "Chennai", ServiceArea = "Anna Nagar, T Nagar, Adyar, Velachery", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });
            var s27 = await EnsureService(new Service { ProviderId = p8.Id, CategoryId = catPest.Id, Title = "Mosquito Fogging Service", Description = "Dense thermal fogging and larvicide spraying for residential societies, balconies, open lawns, and storm drains.", Price = 899m, PriceType = PriceType.Fixed, City = "Chennai", ServiceArea = "Chennai", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(6, 30, 0), WorkEndTime = new TimeSpan(18, 0, 0) });
            var s28 = await EnsureService(new Service { ProviderId = p8.Id, CategoryId = catPest.Id, Title = "Termite Control Treatment", Description = "Drill-fill-seal subterranean anti-termite treatment with genuine Bayer Premise chemical. 5-year written warranty.", Price = 2999m, PriceType = PriceType.Fixed, City = "Chennai", ServiceArea = "Chennai Metro", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1564844536308-c5b5d6e9d1b7?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(17, 0, 0) });

            // ── 9. Pooja Joshi — Beauty & Salon Professional (Rajkot)
            var p9 = await EnsureProvider("pooja.salon@servicehub.com", "Pooja Joshi", "+91 98251 12345", "Rajkot", "Kalawad Road, Rajkot", "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=200&q=80");
            var s29 = await EnsureService(new Service { ProviderId = p9.Id, CategoryId = catSalon.Id, Title = "Salon for Women (Facial, Waxing, Hair Spa)", Description = "Complete pampering package: Lotus herbal fruit facial, honey waxing, L'Oreal steam hair spa, and threading in hygienic privacy.", Price = 1499m, PriceType = PriceType.Fixed, City = "Rajkot", ServiceArea = "Kalawad Road, Yagnik Road, Amin Marg, Race Course", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1560750588-73207b1ef5b8?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });
            var s30 = await EnsureService(new Service { ProviderId = p9.Id, CategoryId = catSalon.Id, Title = "Bridal & Party Makeup Package", Description = "HD bridal makeup with authentic MAC, Kryolan, and Huda Beauty cosmetics. Includes hair design, saree draping, and jewelry fixing.", Price = 5999m, PriceType = PriceType.Fixed, City = "Rajkot", ServiceArea = "Rajkot City & Suburbs", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(6, 0, 0), WorkEndTime = new TimeSpan(22, 0, 0) });
            var s31 = await EnsureService(new Service { ProviderId = p9.Id, CategoryId = catSalon.Id, Title = "Express Head Massage & Hair Treatment", Description = "Warm Ayurvedic oil champi, stress-relief pressure point massage, and warm steam towel therapy at home.", Price = 399m, PriceType = PriceType.Fixed, City = "Rajkot", ServiceArea = "Rajkot", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1519014816548-bf785179c2ff?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 30, 0), WorkEndTime = new TimeSpan(18, 30, 0) });

            // ── 10. Kavita Nambiar — Wellness & Yoga Trainer (Bengaluru)
            var p10 = await EnsureProvider("kavita.yoga@servicehub.com", "Kavita Nambiar", "+91 98455 78901", "Bengaluru", "Koramangala, Bengaluru", "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=200&q=80");
            var s32 = await EnsureService(new Service { ProviderId = p10.Id, CategoryId = catTutor.Id, Title = "Yoga & Personal Fitness Trainer at Home", Description = "Customized morning/evening yoga, pranayama breathing, weight management, and posture alignment sessions at home.", Price = 750m, PriceType = PriceType.Hourly, City = "Bengaluru", ServiceArea = "Koramangala, Indiranagar, HSR Layout, Bellandur", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(6, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });

            // ── 11. Mohammed Khan — Painting & Waterproofing (Mumbai)
            var p11 = await EnsureProvider("mohammed.paint@servicehub.com", "Mohammed Khan", "+91 98220 54321", "Mumbai", "Kurla West, Mumbai", "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=200&q=80");
            var s33 = await EnsureService(new Service { ProviderId = p11.Id, CategoryId = catPaint.Id, Title = "Interior Wall Painting", Description = "Complete interior home painting using Asian Paints Royale & Apcolite. 2 coats of emulsion, wall putty touchups, primer, and floor masking.", Price = 6999m, PriceType = PriceType.Fixed, City = "Mumbai", ServiceArea = "Bandra, Kurla, Chembur, Ghatkopar, Dadar", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1589939705384-5185137a7f0f?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 30, 0), WorkEndTime = new TimeSpan(18, 30, 0) });
            var s34 = await EnsureService(new Service { ProviderId = p11.Id, CategoryId = catPaint.Id, Title = "Exterior Waterproofing & Weather Defense", Description = "Heavy-duty exterior wall and terrace waterproofing coating. Dr. Fixit chemical membrane prevents dampness and cracks.", Price = 12999m, PriceType = PriceType.Fixed, City = "Mumbai", ServiceArea = "Mumbai & Navi Mumbai", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(17, 0, 0) });
            var s35 = await EnsureService(new Service { ProviderId = p11.Id, CategoryId = catPaint.Id, Title = "Texture & Accent Wall Painting", Description = "Designer metallic textures, marble finish, stucco, and geometric stencil walls to elevate living room and bedroom aesthetics.", Price = 3499m, PriceType = PriceType.Fixed, City = "Mumbai", ServiceArea = "Bandra, Juhu, Andheri, Powai, Thane", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1513694203232-719a280e022f?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(18, 0, 0) });

            // ── 12. Rajesh Verma — Woodwork & Carpentry Artisan (Ahmedabad)
            var p12 = await EnsureProvider("rajesh.carpentry@servicehub.com", "Rajesh Verma", "+91 98190 23456", "Ahmedabad", "Navrangpura, Ahmedabad", "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200&q=80");
            var s36 = await EnsureService(new Service { ProviderId = p12.Id, CategoryId = catCarp.Id, Title = "Furniture Assembly & Installation", Description = "Expert assembly of flat-pack furniture from IKEA, Pepperfry, Urban Ladder: wardrobes, king beds, study tables, and TV units.", Price = 599m, PriceType = PriceType.Fixed, City = "Ahmedabad", ServiceArea = "Navrangpura, Bodakdev, Satellite, Vastrapur", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1538688525198-9b88f6f53126?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(19, 0, 0) });
            var s37 = await EnsureService(new Service { ProviderId = p12.Id, CategoryId = catCarp.Id, Title = "Door Lock, Latch & Hinge Repair", Description = "Replacement and alignment of Godrej rim locks, mortise handles, hydraulic door closers, and noisy cupboard hinges.", Price = 349m, PriceType = PriceType.Fixed, City = "Ahmedabad", ServiceArea = "Ahmedabad", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat,Sun", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(20, 0, 0) });
            var s38 = await EnsureService(new Service { ProviderId = p12.Id, CategoryId = catCarp.Id, Title = "Modular Kitchen Repair & Shutter Alignment", Description = "Repair and replacement of soft-close telescopic drawer channels, stainless steel wire baskets, and misaligned hydraulic shutters.", Price = 1299m, PriceType = PriceType.Fixed, City = "Ahmedabad", ServiceArea = "Ahmedabad", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1556911220-e15b29be8c8f?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(9, 0, 0), WorkEndTime = new TimeSpan(18, 30, 0) });

            // ── 13. Sunita Rao — Urban Gardening & Landscaping (Bengaluru)
            var p13 = await EnsureProvider("sunita.gardening@servicehub.com", "Sunita Rao", "+91 98480 34567", "Bengaluru", "HSR Layout, Bengaluru", "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=200&q=80");
            var s39 = await EnsureService(new Service { ProviderId = p13.Id, CategoryId = catGarden.Id, Title = "Lawn Mowing & Maintenance", Description = "Professional electric lawn mowing, grass edge trimming, weed removal, and organic vermicompost feeding for lush turf.", Price = 899m, PriceType = PriceType.Fixed, City = "Bengaluru", ServiceArea = "HSR Layout, Bellandur, Sarjapur Road, Whitefield", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1585320806297-9794b3e4eeae?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(7, 30, 0), WorkEndTime = new TimeSpan(17, 30, 0) });
            var s40 = await EnsureService(new Service { ProviderId = p13.Id, CategoryId = catGarden.Id, Title = "Terrace Garden Setup & Landscaping", Description = "Urban terrace garden layout: lightweight planters, automated drip irrigation kit, rich nutrient soil, and 15 flowering plants.", Price = 3999m, PriceType = PriceType.Fixed, City = "Bengaluru", ServiceArea = "Bengaluru Urban", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1585320806297-9794b3e4eeae?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(17, 0, 0) });
            var s41 = await EnsureService(new Service { ProviderId = p13.Id, CategoryId = catGarden.Id, Title = "Plant Health & Organic Pest Treatment", Description = "Treatment of mealybugs, aphids, and fungus using cold-pressed neem oil, bio-enzymes, and nutrient-rich seaweed tonic.", Price = 699m, PriceType = PriceType.Fixed, City = "Bengaluru", ServiceArea = "HSR Layout, Whitefield, Electronic City", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=800&q=80", WorkDays = "Mon,Tue,Wed,Thu,Fri,Sat", WorkStartTime = new TimeSpan(8, 0, 0), WorkEndTime = new TimeSpan(17, 0, 0) });

            await context.SaveChangesAsync();

            // ── CUSTOMERS ─────────────────────────────────────────────────────────
            async Task<ApplicationUser> EnsureCustomer(string email, string fullName, string phone, string city)
            {
                var existing = await userManager.FindByEmailAsync(email);
                if (existing != null) return existing;

                var c = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    PhoneNumber = phone,
                    City = city,
                    CreatedAt = DateTime.UtcNow
                };
                var r = await userManager.CreateAsync(c, "Customer@123!");
                if (r.Succeeded) await userManager.AddToRoleAsync(c, "Customer");
                return c;
            }

            var c1 = await EnsureCustomer("prince@example.com", "Prince Kumar", "+91 98710 23456", "Mumbai");
            var c2 = await EnsureCustomer("akanchha@example.com", "Akanchha Singh", "+91 98110 34567", "Delhi NCR");
            var c3 = await EnsureCustomer("rohan@example.com", "Rohan Joshi", "+91 98230 56789", "Pune");
            var c4 = await EnsureCustomer("pooja@example.com", "Pooja Nair", "+91 98450 11223", "Bengaluru");
            var c5 = await EnsureCustomer("chirag@example.com", "Chirag Mehta", "+91 98252 66778", "Rajkot");

            // ── SAMPLE BOOKINGS & REVIEWS ─────────────────────────────────────────
            async Task SeedBookingAndReview(ApplicationUser customer, Service service, int daysAgo, int rating, string reviewComment, decimal amount)
            {
                if (await context.Reviews.AnyAsync(r => r.ServiceId == service.Id && r.CustomerId == customer.Id))
                    return;

                var b = new Booking
                {
                    CustomerId = customer.Id,
                    ServiceId = service.Id,
                    BookingDate = DateTime.Today.AddDays(-daysAgo),
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Status = BookingStatus.Completed,
                    TotalAmount = amount,
                    Notes = "Service booked online and fulfilled promptly.",
                    CreatedAt = DateTime.UtcNow.AddDays(-daysAgo - 1)
                };
                context.Bookings.Add(b);
                await context.SaveChangesAsync();

                var rev = new Review
                {
                    BookingId = b.Id,
                    CustomerId = customer.Id,
                    ServiceId = service.Id,
                    Rating = rating,
                    Comment = reviewComment,
                    ReviewDate = DateTime.UtcNow.AddDays(-daysAgo)
                };
                context.Reviews.Add(rev);
                await context.SaveChangesAsync();
            }

            await SeedBookingAndReview(c1, s1, 12, 5, "Ramesh ji arrived right on time with all required tools. Both ceiling fans were installed with precision. Outstanding work!", 349m);
            await SeedBookingAndReview(c2, s5, 10, 5, "Amit found the hidden leakage in our bathroom pipeline quickly and repaired it cleanly without damaging extra tiles. Very satisfied!", 399m);
            await SeedBookingAndReview(c4, s10, 8, 5, "Priya and her cleaning team did an incredible job with our 3BHK deep cleaning. The kitchen and bathrooms look brand new!", 2499m);
            await SeedBookingAndReview(c3, s14, 15, 5, "Suresh sir explained the calculus concepts so clearly to my son. His scores in the recent pre-board mock exam improved significantly.", 1600m);
            await SeedBookingAndReview(c4, s17, 6, 5, "Rahul was courteous and fixed the spinning drum issue of our LG washing machine within 45 minutes using genuine spare parts.", 699m);
            await SeedBookingAndReview(c5, s20, 5, 5, "Bhavik did fantastic smart home setup in our Rajkot apartment. Voice controls work flawlessly!", 5999m);
            await SeedBookingAndReview(c5, s29, 3, 5, "Pooja provided top quality salon service at home in Rajkot. Super hygienic and relaxing!", 1499m);
            await SeedBookingAndReview(c1, s33, 14, 5, "Mohammed bhai and his crew completed interior painting for our 2BHK on schedule with zero mess. Beautiful finish!", 6999m);
            await SeedBookingAndReview(c2, s26, 7, 5, "Karthik did the gel pest control treatment. Highly effective, completely odorless, and no cockroaches seen since!", 699m);
            await SeedBookingAndReview(c4, s32, 9, 5, "Kavita's yoga sessions have made a huge difference to my posture and energy levels. Highly recommended!", 1500m);
            await SeedBookingAndReview(c3, s16, 13, 5, "Excellent Python fundamentals tutoring by Suresh. Very practical and structured teaching.", 1800m);
            await SeedBookingAndReview(c4, s39, 4, 5, "Sunita manicured our lawn and pruned the hedges wonderfully. Our terrace looks fresh and green!", 899m);
        }
    }
}
