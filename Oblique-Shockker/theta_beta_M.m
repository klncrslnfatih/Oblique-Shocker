close all; clear all; clc
g = 1.4;                                 % Specific heat ratio for air
beta = 0:(pi/180):(pi/2);                % Range for shock wave angle
m = 0;    
                                         % theta (=flow deflection angle)
for M1 = 1:0.01:5                        % Upstream Mach Number
    m = m+1;
    %================================
    %     theta-beta-M relation
    %================================
    Nr = ((M1^2)*((sin(beta)).^2))-1;    
    Dr = ((g+(cos(2*beta)))*M1^2)+2;
    theta = atan(2*cot(beta).*Nr./Dr);
    %================================
    %      max. theta for a M1
    %================================
    a(m) = max(theta);                   % max theta for the Mach No.
    b(m) = beta(find(theta==a(m)));      % find the beta for max. theta
    plot(theta,beta,'-b')
    hold on
end
plot(a,b,'-r','Linewidth',1.5)
xlabel('\theta')
ylabel('\beta')
axis([0 42*pi/180 0 pi/2])