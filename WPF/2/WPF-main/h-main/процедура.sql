CREATE OR ALTER PROCEDURE UpdateStatusesAutomatically
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Edinica
    SET FK_Status = 2 
    WHERE FK_Status = 1 
        AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 90;
    UPDATE Edinica
    SET FK_Status = 5 
    WHERE FK_Status = 2 
        AND Component = 'Эритроциты'
        AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 180;
    UPDATE Edinica
    SET FK_Status = 5 
    WHERE FK_Status = 2
        AND Component = 'Плазма'
        AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 240;
END;