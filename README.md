# Book Lending System

## **Description**:
 A lightweight library/book-lending application that manages books, copies, members, memberships, borrowings, returns, payments and fines.

## **Purpose**: 
Manage library workflows for admins and members; enforce membership and fine business rules.

## **Admin workflow**: 
manage categories, books and copies; register and manage members and memberships; configure fine rules; approve borrow requests and mark copies lent; process returns, record damages and payments; view reports and pending fines.
## **Member workflow**: 
search and view available books and categories; request borrow for an available copy; view borrow history, active loans and unpaid fines; renew loan when allowed by membership rules; return books and pay fines.
## **Business rules**: 
- member must be registered and active to borrow.
- unpaid fine threshold that blocks borrowing is ₹500.
- borrow count must be within membership.MaxBooksAllowed.
- borrow duration uses membership.MaxBorrowDurationDays; due date = borrowDate + duration.
- a member cannot have more than one active borrow for the same book.
- book copy must be in status Available or Damaged to be borrowed; returning sets status to Damaged if damagePercentage > 0, otherwise Available.
- late fee calculation uses configured fine rules (PerDay, FlatFee, Percentage); default fallback is ₹10 per day if no rule configured.
- returns compute delayedDays = max(0, returnDate - dueDate) and create a late fee payment when fine > 0.
## **Implementation** : 
borrow validations are performed before opening a transaction and some calculations use database functions (calculate_member_fine, get_member_borrowing_summary).
## **Project summary**: 
Modular .NET solution with projects: `BookLendingApp.FEApplication` (frontend/console), `BookLendingApp.BALLibrary` (business logic), `BookLendingApp.DALLibrary` (data access) and `BookLendingApp.ModelLibrary` (models/DTOs/enums).
## **Key entities**: 
Member, Membership, Book, BookCopy, BorrowRecord, Payment, FineRule, Category.
## **Data flow**: 
Member actions hit FE, BE validations in `BorrowingService.cs`, persistence via repositories in `BookLendingApp.DALLibrary`.
## **Admin responsibilities**: 
create/edit categories and books; add physical copies and set initial `BookStatus`; create memberships with `MaxBooksAllowed`, `MaxBorrowDurationDays`, renewal settings and fees; create or update `FineRule` entries; run reports for most-borrowed books and members with pending fines.
## **Admin step**: 
- register member -> assign membership -> activation flag.
- add book -> add copies -> set copy `Status` to `Available`.
-  configure fine rules -> types: `LateReturn` with `FineCalculationType` (PerDay, FlatFee, Percentage) and amounts.
- view pending fines -> collect payments -> mark payments via `IPaymentRepository`.
- keep account active, pay fines timely, respect membership limits and renewal policy.
-  search catalog by category or title -> view available copies count -> request borrow for a copy.
- on borrow request, system validates: member active, unpaid fine <= ₹500, active borrow count < `MaxBooksAllowed`, no active borrow for same book, and copy status is borrowable.
- when borrowing succeeds, `BorrowRecord` created, copy `Status` set to `LentOut`, `BorrowDate` stored and `RenewalDays` set from membership.
-  to renew, system checks `IsRenewalAllowed` and `RenewalCount < MaxRenewalTimes`; renewal updates `RenewalCount` and extends `RenewalDays`.
-  on return, compute `delayedDays = max(0, (returnDate - dueDate).Days)` -> compute fine via configured `FineRule` or fallback -> create `LateFeePayment` if fine > 0 -> update `BorrowRecord` and copy `Status`.
## **Business rule**: 
- unpaid fines are retrieved using DB function `calculate_member_fine` when available, with fallback to repository aggregation.
- borrowing is blocked when unpaid fines > ₹500; message returned to user explains the block.
- only `BookStatus.Available` or `BookStatus.Damaged` are allowed for creating a borrow; other statuses are rejected.
-  damage handling: returns can include `damagePercentage`; if > 0 then copy `Status` becomes `Damaged` and damage is recorded on the borrow record.
-  transactional safety: create/update of `BorrowRecord` and `BookCopy` status changes are wrapped in DB transactions in `BorrowingService`.
## **Reporting**: 
`GetMostBorrowedBooks`, `GetMembersWithPendingFines`, `GetAvailableBooksByCategory` are implemented in `BorrowingService.cs` and rely on repository data or DB functions.
