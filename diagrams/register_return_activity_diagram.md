@startuml
title Registrera återlämning

start

repeat :Visar bokningar;
:Väljer rätt bokning;
:Visar bokningsuppgifter;
:Kontrollerar det återlämnade exemplarets skick;

if (Exemplaret är i godtagbart skick) then(NEJ)
  fork
  fork again
    :Markera exemplaret som otillgängligt;
    end
  end fork
else (JA)
  
endif

:Kontrollerar att inventarienummret på exemplaret stämmer\növerrens med inventarienummret i bokningen;

repeat while (Fel bokning vald) is (JA) not (NEJ)

if (Inventarienummer stämmer överrens med utlämnat exemplar) then(JA)
  :Bekräftar om återlämningen ska slutföras;
else(NEJ)
  :Felmeddelande visas;
  :Återlämningen avbryts;
  end
endif

if (Återlämningen ska slutföras) then(JA)

  repeat :Kontrollera att personalen är inloggad;
    backward :Personalen får logga in;
  repeat while (Är personalen inloggad) is (NEJ) not (JA)
  
else(NEJ)
  :Återlämningen avbryts;
  end
  
endif

if (Är exemplaret redan registrerat som återlämnat?) then(NEJ)
  :Spara återlämninen med koppling till rätt\nexemplar och registrerande personal;
  :Visa bekräftelse på registrerad återlämning;
  
else(JA)
  :Återlämningen avbryts;
  end
  
endif

stop
@enduml